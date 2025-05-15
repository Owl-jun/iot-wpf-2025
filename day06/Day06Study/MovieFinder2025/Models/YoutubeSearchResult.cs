using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieFinder2025.Models
{
    public partial class YoutubeSearchResult : ObservableObject
    {
        [ObservableProperty]
        private List<Item> _items;

        public class Item
        {
            public Id Id { get; set; }
            public Snippet Snippet { get; set; }
        }

        public class Id
        {
            public string Kind { get; set; }
            public string VideoId { get; set; }
        }

        public class Snippet
        {
            public string Title { get; set; }
            public string ChannelTitle { get; set; }
            public string Description { get; set; }
            public ThumbnailGroup Thumbnails { get; set; }
        }

        public class ThumbnailGroup
        {
            public ThumbnailInfo Default { get; set; }
            public ThumbnailInfo Medium { get; set; }
            public ThumbnailInfo High { get; set; }
        }

        public class ThumbnailInfo
        {
            public string Url { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }
    }
}
