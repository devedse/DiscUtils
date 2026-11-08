using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DiscUtils.Ntfs;
using DiscUtils.Streams;

namespace LibraryTests.Ntfs.Journal;

/// <summary>
/// A power cut can stop a replay after any of its writes, or in the middle of one (the disk then holds the first
/// sectors of that write and the old contents after them). Each Windows crash sample is replayed with such a cut and
/// then replayed again. The result must be what Windows' own replay of the crash gives. The only exception is a cut
/// that left a page half written (a torn write, or a cut between the clusters of one file record or index buffer)
/// while the log still has to change that page: no replay can rebuild it, so the retry must refuse without writing
/// and leave the volume to Windows.
/// </summary>
public class NtfsJournalWindowsInterruptionTest
{
    [Theory]
    [MemberData(nameof(NtfsJournalWindowsSampleTest.Samples), MemberType = typeof(NtfsJournalWindowsSampleTest))]
    public void APowerCutDuringReplay_LeavesAVolumeThatReplaysToWhatWindowsGives(string name, NtfsJournalStatus expected, int clusterSize)
    {
        var image = NtfsJournalWindowsSampleTest.LoadVolume(name).ToArray();
        var digest = NtfsJournalWindowsSampleTest.LoadDigest(name);

        List<PowerCutStream.WriteInfo> writes;
        using (var uninterrupted = Overlay(image))
        {
            var counter = new PowerCutStream(uninterrupted, int.MaxValue, torn: false);
            Assert.Equal(expected, NtfsJournal.Replay(counter).Status);
            writes = counter.Writes;
        }

        // Both restart page writes of the reset, the first and last metadata write, and one in between.
        var resets = writes.Select((w, i) => (w, i)).Where(x => x.w.IsRestartPage).Select(x => x.i).ToList();
        Assert.Equal(2, resets.Count);
        Assert.NotEqual(writes[resets[0]].Position, writes[resets[1]].Position);
        var cuts = new SortedSet<int>(resets);
        if (resets[0] > 0)
        {
            cuts.UnionWith([0, resets[0] / 2, resets[0] - 1]);
        }

        var refusals = 0;
        foreach (var cut in cuts)
        {
            foreach (var torn in new[] { false, true })
            {
                if (torn && writes[cut].Length <= 512)
                {
                    continue;
                }

                using var volume = Overlay(image);
                var power = new PowerCutStream(volume, cut, torn);
                Assert.Throws<IOException>(() => NtfsJournal.Replay(power));
                Assert.True(power.Dead);

                var what = $"{name}: power cut at write {cut} ({writes[cut].Kind}{(torn ? ", torn" : "")})";
                var state = NtfsJournal.Analyze(volume);
                if (state.Status == NtfsJournalStatus.Unsupported)
                {
                    // Only a page the cut left half written: a torn write, or a cut between the clusters of one page.
                    Assert.True(state.Detail.Contains("damaged") || state.Detail.Contains("torn"), $"{what}: the retry refuses: {state.Detail}");
                    var refused = new PowerCutStream(volume, 0, torn: false);
                    Assert.Throws<IOException>(() => NtfsJournal.Replay(refused));
                    Assert.False(refused.Dead, $"{what}: the refused retry wrote");
                    refusals++;
                    continue;
                }

                NtfsJournal.Replay(volume);
                var mismatches = WindowsReplayDigest.Check(volume, digest);
                Assert.True(mismatches.Count == 0, $"{what}: {mismatches.Count} differences from Windows' replay, first: {mismatches.FirstOrDefault()}");
                Assert.Equal(NtfsJournalStatus.Clean, NtfsJournal.Analyze(volume).Status);
            }
        }

        Assert.True(refusals < cuts.Count, $"{name}: every cut was refused");
        Assert.True(clusterSize > 0);
    }

    // The sample as a read-only base with a copy-on-write layer on top, so every attempt starts from the same crash.
    private static SnapshotStream Overlay(byte[] image)
    {
        var overlay = new SnapshotStream(new MemoryStream(image, writable: false), Ownership.Dispose);
        overlay.Snapshot();
        return overlay;
    }

    /// <summary>Passes writes on until write number <c>cutAt</c>, which fails (after its first half when torn).</summary>
    private sealed class PowerCutStream(Stream inner, int cutAt, bool torn) : Stream
    {
        internal readonly record struct WriteInfo(long Position, int Length, string Kind)
        {
            public bool IsRestartPage => Kind == "RSTR";
        }

        public List<WriteInfo> Writes { get; } = [];

        public bool Dead { get; private set; }

        public override bool CanRead => inner.CanRead;

        public override bool CanSeek => inner.CanSeek;

        public override bool CanWrite => inner.CanWrite;

        public override long Length => inner.Length;

        public override long Position
        {
            get => inner.Position;
            set => inner.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count) => inner.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => inner.Seek(offset, origin);

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Flush()
        {
            if (Dead)
            {
                throw new IOException("The power is off");
            }

            inner.Flush();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (Dead)
            {
                throw new IOException("The power is off");
            }

            if (Writes.Count == cutAt)
            {
                Dead = true;
                if (torn)
                {
                    // The disk got the first half of this write, in whole sectors.
                    inner.Write(buffer, offset, count / 2 / 512 * 512);
                }

                throw new IOException("Power cut");
            }

            var kind = count >= 4 ? System.Text.Encoding.ASCII.GetString(buffer, offset, 4) : "?";
            Writes.Add(new(inner.Position, count, kind is "FILE" or "INDX" or "RSTR" ? kind : "other"));
            inner.Write(buffer, offset, count);
        }
    }
}
