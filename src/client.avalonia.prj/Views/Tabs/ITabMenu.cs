namespace Client.Avalonia.Views
{
    /// <summary>
    /// Тип содержимого отображаемой вкладки.
    /// </summary>
    public enum TabCategoryEnum
    {
        /// <summary>
        /// Графики.
        /// </summary>
        Graphs,
        /// <summary>
        /// Графический редактор.
        /// </summary>
        GraphicEditor
    }

    /// <summary>
    /// Интерфейс для вкладок в меню.
    /// </summary>
    public interface ITabMenu
    {
        /// <summary>
        /// Идентификатор. Связывает ITabMenu и ITabVM.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Выбрана ли вкладка?
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Заголовок страницы.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Тип отображаемой информации данной вкладкой.
        /// </summary>
        TabCategoryEnum TabCategory { get; }
    }
}
