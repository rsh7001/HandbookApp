using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandbookApp.States;
using Xamarin.Forms;

namespace HandbookApp.Views
{
    public partial class ArticleItem : ViewCell, INotifyPropertyChanged
    {
        public static readonly BindableProperty ArticleProperty = BindableProperty.Create(
            propertyName: "ArticleProperty",
            returnType: typeof(Article),
            declaringType: typeof(ArticleItem),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay);

        public Article Article
        {
            get
            {
                return (Article)GetValue(ArticleProperty);
            }
            set
            {
                SetValue(ArticleProperty, value);
                OnPropertyChanged();
            }
        }

        public ArticleItem()
        {
            InitializeComponent();
        }
    }
}
