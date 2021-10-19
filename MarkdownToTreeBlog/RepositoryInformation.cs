using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarkdownToTreeBlog
{
    class RepositoryInformation : IDisposable
    {
        public static RepositoryInformation GetRepositoryInformationForPath(string path)
        {
            if (LibGit2Sharp.Repository.IsValid(path))
            {
                return new RepositoryInformation(path);
            }
            return null;
        }

        public string CommitHash
        {
            get
            {
                return _repo.Head.Tip.Sha;
            }
        }

        public string BranchName
        {
            get
            {
                return _repo.Head.CanonicalName;
            }
        }

        public string TrackedBranchName
        {
            get
            {
                return _repo.Head.IsTracking ? _repo.Head.TrackedBranch.CanonicalName : String.Empty;
            }
        }

        public bool HasUnpushedCommits
        {
            get
            {
                return _repo.Head.TrackingDetails.AheadBy > 0;
            }
        }

        public bool HasUncommittedChanges
        {
            get
            {
                return _repo.RetrieveStatus().Any(s => s.State != FileStatus.Ignored);
            }
        }

        public IEnumerable<Commit> Log
        {
            get
            {
                return _repo.Head.Commits;
            }
        }

        public DateTime? GetFileLastCommitTime(string path)
        {
            var logs = _repo.Commits.QueryBy(path).Take(2).ToList();
            if (!logs.Any() || logs.Count < 2) return null;

            var patch = _repo.Diff.Compare<Patch>(logs[1].Commit.Tree, logs[0].Commit.Tree);
            PatchEntryChanges entryChanges = patch[path];
            if (entryChanges.Status != ChangeKind.Modified) return null;

            return logs[0].Commit.Author.When.LocalDateTime;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _repo.Dispose();
            }
        }

        private RepositoryInformation(string path)
        {
            _repo = new Repository(path);
        }

        private bool _disposed;
        private readonly Repository _repo;
    }
}