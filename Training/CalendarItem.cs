using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Training
{
    public class CalendarItem
    {
        public int Id { set; get; }
        public string Title { set; get; }
        public DateTime StartDate { set; get; }
        public DateTime EndDate { set; get; }

        public IEnumerable<CalendarItem> MaybeSplit(int currentYear)
        {
            ValidateDates();

            if ( GetNumberOfYears() > 0)
            {
                return Split(currentYear);
            }
            else
            {
                ICollection<CalendarItem> splitCalendarItems = new List<CalendarItem>();
                splitCalendarItems.Add(this);
                return splitCalendarItems.ToArray();
            }
        }

        private int GetNumberOfYears()
        {
            var numberOfYearsBetween = EndDate.Year - StartDate.Year;
            return numberOfYearsBetween;
        }

        private IEnumerable<CalendarItem> Split(int currentYear)
        {
            ICollection<CalendarItem> splitCalendarItems = new List<CalendarItem>();

            var dateTimeKind = GetDateTimeKind();

            DateTime splitStartDate = StartDate;
            var splitEndDate = GetEndOfYear(splitStartDate.Year, dateTimeKind);
            for (int numberOfSplits = 0; numberOfSplits < GetNumberOfYears(); numberOfSplits++)
            {
                splitCalendarItems.Add(CloneCalendarItemForPeriod(splitStartDate,splitEndDate));
                splitStartDate = GetStartOfYear(splitEndDate.Year + 1, dateTimeKind);
                splitEndDate = GetEndOfYear(splitStartDate.Year, dateTimeKind);
            }

            splitCalendarItems.Add(CloneCalendarItemForPeriod(splitStartDate,EndDate));

            return splitCalendarItems.ToArray();
        }

        private CalendarItem CloneCalendarItemForPeriod(DateTime startDate, DateTime endDate)
        {
            return new CalendarItem
            {
                Id = Id,
                Title = Title,
                StartDate = startDate,
                EndDate = endDate,
            };
        }

        private DateTime GetEndOfYear(int year, DateTimeKind dateTimeKind)
        {
            return new DateTime(year, 12, 31, 23, 59, 0, dateTimeKind);
        }

        private DateTime GetStartOfYear(int year, DateTimeKind dateTimeKind)
        {
            return new DateTime(year, 1, 1, 0, 0, 0, dateTimeKind);
        }

        private DateTimeKind GetDateTimeKind()
        {
            return StartDate.Kind;
        }

        private bool ValidateDates()
        {
            if (EndDate < StartDate)
            {
                throw  new InvalidConstraintException("EndDate should be bigger than StartDate");
            }
            if (EndDate.Kind != StartDate.Kind)
            {
                throw new InvalidConstraintException("EndDate and StartDate should have the same DateTimeKind");
            }
            return true;
        }
    }}
