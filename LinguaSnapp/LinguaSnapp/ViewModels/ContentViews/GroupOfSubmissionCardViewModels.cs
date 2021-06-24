using DotNetExtensions;
using LinguaSnapp.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace LinguaSnapp.ViewModels.ContentViews
{
    class GroupOfSubmissionCardViewModels : List<SubmissionCardViewModel>, IComparable<GroupOfSubmissionCardViewModels>
    {
        public string LongName { get; }

        public string ShortName { get; }

        public SubmissionStatus Status { get; }

        internal GroupOfSubmissionCardViewModels(SubmissionStatus status)
        {
            Status = status;
            LongName = status.GetDescription();
            ShortName = LongName;
        }

        public int CompareTo(GroupOfSubmissionCardViewModels other)
        {
            return Status.CompareTo(other.Status);
        }
    }
}
