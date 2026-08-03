using Avalonia.Media;
using ReactiveUI;
using System; 

namespace Lib.Avalonia.Controls.Helpers
{
    /// <summary>
    /// Объект, хранящий информацию о событии в планировщике.
    /// </summary>
    public class SchedulerEventItem : ReactiveObject, ISchedulerEventItem
    {
        private DateTime _startDate;
        private DateTime _endDate;
        private string _name;
        private string _description;
        private IBrush _color;

        public Guid Id { get; }

        /// <inheritdoc/>
        public DateTime StartDate
        {
            get => _startDate;
            private set => this.RaiseAndSetIfChanged(ref _startDate, value);
        }

        /// <inheritdoc/>
        public DateTime EndDate
        {
            get => _endDate;
            private set => this.RaiseAndSetIfChanged(ref _endDate, value);
        }

        /// <inheritdoc/>
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        /// <inheritdoc/>
        public string Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }

        /// <inheritdoc/>
        public IBrush Color
        {
            get => _color;
            set => this.RaiseAndSetIfChanged(ref _color, value);
        }

        public SchedulerEventItem( 
            DateTime startDate,
            DateTime endDate,
            string name,
            string description,
            IBrush color,
            Guid? guid = null)
        {
            Id          = guid ?? Guid.NewGuid();
            StartDate   = startDate;
            EndDate     = endDate;
            Name        = name;
            Description = description;
            Color       = color;
        }
         
        /// <inheritdoc/>
        public ISchedulerEventItem Clone()
        {
            return new SchedulerEventItem(
                StartDate,
                EndDate,
                Name,
                Description,
                Color,
                Id);
        }

        /// <inheritdoc/>
        public void ChangeDateTimeRange(DateTime start, DateTime end)
        {
            StartDate = start;
            EndDate   = end;
        }

    }
}
