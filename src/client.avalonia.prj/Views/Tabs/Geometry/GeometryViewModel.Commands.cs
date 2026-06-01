using Avalonia;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.Avalonia.Views.Geometry
{
    public sealed partial class GeometryViewModel
    {
        public sealed class GeometryViewModelCommands
        {
            public ReactiveCommand<Point, Unit> CreateShapeCommand { get; }
            public ICommand RemoveShapeCommand { get; }

            public GeometryViewModelCommands(GeometryViewModel vm)
            {
                CreateShapeCommand = ReactiveCommand.Create<Point>(vm.CreateShape);
                RemoveShapeCommand = ReactiveCommand.Create(vm.RemoveSelectedShape);
            }
        }

        private GeometryViewModelCommands? _commands;

        public GeometryViewModelCommands Commands => _commands ??= new(this);

        /// <summary>
        /// Создание новой геом-фигуры.
        /// </summary> 
        private void CreateShape(Point point)
        {
            var newShape = GeometryCreateMenuViewModel.CurrentShapeCreaterVM.Create(point.X, point.Y);
            if(newShape != null)
            { 
                _shapeService.AddShape(newShape);
            }
        }

        /// <summary>
        /// Удаление текущей выбранной геом. фигуры.
        /// </summary>
        private void RemoveSelectedShape()
        {
            var currentShape = _shapeService.GetCurrentSelectedShape();
            if(currentShape != null)
            { 
                _shapeService.RemoveShapeById(currentShape.Id);
            } 
        }
    }
}
