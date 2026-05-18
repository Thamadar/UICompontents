using Client.WPF.Views.Geometry.Shapes;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.WPF.Services.Interfaces
{
    //TO DO: IDisposable привентить.
    /// <summary>
    /// Сервис управления геом. фигурами в проекте между VM.
    /// </summary>
    public interface IShapeService
    {
        /// <summary> 
        /// Подключение к списку всех геом. фигур.
        /// </summary> 
        IObservable<IChangeSet<IShapeItem>> ConnectToTotalShapes();

        /// <summary>
        /// Отслеживание текущей выбранной геом. фигуры.
        /// </summary>
        IObservable<IShapeItem?> CurrentSelectedShapeObservable { get; }

        /// <summary>
        /// Получить текущую выделенную геом. фигуру.
        /// </summary> 
        IShapeItem? GetCurrentSelectedShape();

        /// <summary>
        /// Добавить геом. фигуру
        /// </summary>
        void AddShape(IShapeItem shapeItem);

        /// <summary>
        /// Добавление множество геом. фигур.
        /// </summary> 
        void AddRangeShape(IEnumerable<IShapeItem> shapeItems);

        /// <summary>
        /// Удалить геом. фигуру.
        /// </summary> 
        void RemoveShapeById(Guid guid);

        /// <summary>
        /// Удалить все геом. фигуры.
        /// </summary>
        void RemoveAllShapes();

        /// <summary>
        /// Выбрать геом. фигуру по ID.
        /// </summary> 
        void SelectShapeById(Guid? guid = null);
    }
}
