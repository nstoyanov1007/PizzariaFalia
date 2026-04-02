using System;
using System.Collections.Generic;

namespace PizzariaFalia.ViewModels
{
    public class PaginatedListViewModel<T> //Viewmodel for pagination purposes
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();

        public int PageIndex { get; set; }
        public int TotalPages { get; set; }

        public bool HasPrevious => PageIndex > 1;
        public bool HasNext => PageIndex < TotalPages;
    }
}