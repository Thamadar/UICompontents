using Avalonia;
using Lib.Avalonia;
using Lib.Avalonia.Extensions;
using Lib.Avalonia.Helpers;
using ReactiveUI; 
using System.Collections.ObjectModel;
using System.Diagnostics; 

namespace Client.Avalonia.Views.Geometry
{
    public class GeometryManagementPanelViewModel : ViewModelBase
    {
        /// <summary>
        /// Список меню управления.
        /// </summary>
        public ObservableCollection<IMenuDataItem> MenuItems { get; }   

        public GeometryManagementPanelViewModel()
        {
            MenuItems = new ObservableCollection<IMenuDataItem>();

            LoadMenuItems();
        }
         
        private void LoadMenuItems()
        { 
            var userIcon     = Application.Current?.GetTemplateResource("TestIcon");
            var settingsIcon = Application.Current?.GetTemplateResource("TestIcon");
            var fileIcon     = Application.Current?.GetTemplateResource("TestIcon");
                
            var fileChilds = new List<MenuDataItem>
            {
                new MenuDataItem(
                text: "Новый...",
                icon: fileIcon,
                command: ReactiveCommand.Create(() => Debug.WriteLine("Новый файл")),
                key: "Ctrl+N"),

                new MenuDataItem(
                text: "Открыть...",
                icon: fileIcon,
                command: ReactiveCommand.Create(() => Debug.WriteLine("Открыть файл")),
                key: "Ctrl+O"),
                 
                new MenuDataItem(
                text: "Последние файлы",
                icon: fileIcon,
                childs: new List<MenuDataItem>()
                {
                     new MenuDataItem(
                        text: "0 Project0.PSD", 
                        command: ReactiveCommand.Create(() => Debug.WriteLine("Project0.PSD"))),
                       new MenuDataItem(
                        text: "1 Project1.PSD",
                        command: ReactiveCommand.Create(() => Debug.WriteLine("Project1.PSD"))),
                         new MenuDataItem(
                        text: "2 Project2.PSD",
                        command: ReactiveCommand.Create(() => Debug.WriteLine("Project2.PSD"))),
                           new MenuDataItem(
                        text: "3 Project3.PSD",
                        command: ReactiveCommand.Create(() => Debug.WriteLine("Project3.PSD"))),
                          new MenuDataItem(
                        text: "4 Project4.PSD",
                        command: ReactiveCommand.Create(() => Debug.WriteLine("Project4.PSD"))),
                            new MenuDataItem(
                        text: "5 Project5.PSD",
                        command: ReactiveCommand.Create(() => Debug.WriteLine("Project5.PSD"))),
                              new MenuDataItem(
                        text: "6 Project6.PSD",
                        command: ReactiveCommand.Create(() => Debug.WriteLine("Project6.PSD"))),
                }),

                new MenuDataItem(isSeparator:true),

                new MenuDataItem(
                text: "Сохранить...",
                icon: fileIcon,
                command: ReactiveCommand.Create(() => Debug.WriteLine("SAVE")),
                key: "Ctrl+S"),

                new MenuDataItem(isSeparator:true),

                new MenuDataItem(
                text: "Выход", 
                command: ReactiveCommand.Create(() => Debug.WriteLine("Выход")))
            };  
            var fileMenu = new MenuDataItem(
                text: "Файл",
                childs: fileChilds);  

            var editChilds = new List<MenuDataItem>
            { 
                new MenuDataItem(
                text: "Отменить",
                icon: fileIcon,
                key: "Ctrl+Z",
                command: ReactiveCommand.Create(() => Debug.WriteLine("Отменить"))),

                new MenuDataItem(
                text: "Вернуть",
                icon: fileIcon,
                key: "Ctrl+Y",
                command: ReactiveCommand.Create(() => Debug.WriteLine("Вернуть"))),

                new MenuDataItem(isSeparator:true),

                new MenuDataItem(
                text: "Клонировать",
                icon: fileIcon,
                key: "Ctrl+J",
                command: ReactiveCommand.Create(() => Debug.WriteLine("Клонировать")))
            }; 
            var editMenu = new MenuDataItem(
                text: "Редактировать",
                icon: fileIcon,
                childs: editChilds);

            var aboutMenu = new MenuDataItem(
                text: "Справка",
                command: ReactiveCommand.Create(() => Debug.WriteLine("Справка")));

            MenuItems.Add(fileMenu);
            MenuItems.Add(editMenu);
            MenuItems.Add(aboutMenu);
        }
    }
}
