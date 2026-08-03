using Avalonia.Media;

using System; 

namespace Lib.Avalonia.Controls.Helpers
{ 
    /// <summary>
    /// Интерфейс объекта, хранящего информацию о событии в планировщике.
    /// </summary>
    public interface ISchedulerEventItem
    {

        Guid Id { get; }

        /// <summary>
        /// Дата начала события.
        /// </summary>
        DateTime StartDate { get; }

        /// <summary>
        /// Дата окончания события.
        /// </summary>
        DateTime EndDate { get; }

        /// <summary>
        /// Наименование события.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Описание события.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Цвет события.
        /// </summary>
        IBrush Color { get; set; }

        /// <summary>
        /// Клонировать.
        /// </summary> 
        ISchedulerEventItem Clone();

        /// <summary>
        /// Обновление временного промежутка.
        /// </summary> 
        void ChangeDateTimeRange(DateTime start, DateTime end);
    }
}
