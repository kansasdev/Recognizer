using System;
using System.Collections.Generic;
using System.Text;

namespace Recognizer.Models
{
    public enum MenuItemType
    {
        Recognize,
        Configuration
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
