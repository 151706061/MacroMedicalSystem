using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Macro.Web.Client.Silverlight.ViewModel;

namespace Macro.Web.Client.Silverlight
{
    public class CheckComboBox : ComboBox
    {
        public class CheckComboBoxItem : ViewModelBase
        {
            private bool _isChecked;
            public bool IsChecked
            {
                get { return _isChecked; }
                set
                {
                    if (value == _isChecked) return;
                    _isChecked = value;
                    if (CheckChanged != null)
                        CheckChanged(this, null);
                    RaisePropertyChanged(() => IsChecked);
                }
            }

            private string _text;
            public string Text
            {
                get { return _text; }
                set
                {
                    if (_text == value) return;
                    _text = value;
                    RaisePropertyChanged(() => Text);
                }
            }

            public event EventHandler<EventArgs> CheckChanged;
        }

        private ContentPresenter _selectedContent;
        private bool _inClear = false;

        public CheckComboBox()
        {
            DefaultStyleKey = typeof (ComboBox);
        }

        public override void OnApplyTemplate()
        {
            _selectedContent = GetTemplateChild("ContentPresenter") as ContentPresenter;
            RefreshContent();
            base.OnApplyTemplate();
            SelectionChanged += (s, e) =>
                                         {
                                             //Cancel selection
                                             SelectedItem = null;
                                             RefreshContent();
                                         };            
        }

        public string[] ItemsSourceArray
        {
            get
            {
                return (string[])GetValue(ItemsSourceArrayProperty);
            }
            set
            {
                SetValue(ItemsSourceArrayProperty, value);
            }
        }

        public string[] SelectedTextArray
        {
            get
            {
                return (string[])GetValue(SelectedTextArrayProperty);
            }
            set
            {
                SetValue(SelectedTextArrayProperty, value);
            }
        }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof (string), typeof (CheckComboBox),
                                        new PropertyMetadata(null,
                                                             (s, e) => ((CheckComboBox) s).RefreshContent()));

        public static readonly DependencyProperty ItemsSourceArrayProperty =
            DependencyProperty.Register("ItemsSourceArray", typeof(string[]), typeof(CheckComboBox),
                                        new PropertyMetadata(null,
                                                             (s, e) => ((CheckComboBox)s).RefreshList()));

        public static readonly DependencyProperty SelectedTextArrayProperty =
            DependencyProperty.Register("SelectedTextArray", typeof (string[]), typeof (CheckComboBox),
                                        new PropertyMetadata(null));

        private void RefreshContent()
        {
            if (_selectedContent != null)
            {
                var tb = (TextBlock) _selectedContent.Content;
                tb.Text = Text;
            }
        }

        private void RefreshList()
        {
            string[] list = ItemsSourceArray;
            var items = new List<CheckComboBoxItem>();
            items.Add(new CheckComboBoxItem {IsChecked = false, Text = SR.Clear});
            foreach (string s in list)
            {
                items.Add(new CheckComboBoxItem {IsChecked = false, Text = s});
            }

            RefreshSelectedItemsText();
            foreach (var item in items)
                item.CheckChanged += (s, e) => RefreshSelectedItemsText();

            ItemsSource = items;
            SelectedTextArray = new string[0];
            Text = string.Empty;
        }


        private void RefreshSelectedItemsText()
        {
            if (_inClear) return;

            if (ItemsSource == null)
            {
                Text = string.Empty;
                return;
            }

            // Check if "Clear" is selected
            foreach (CheckComboBoxItem item in ItemsSource)
            {
                if (item.IsChecked && item.Text.Equals(SR.Clear))
                {
                    _inClear = true;
                    break;
                }
            }

            if (_inClear)
            {
                foreach (CheckComboBoxItem item in ItemsSource)
                {
                    item.IsChecked = false;
                }
                SelectedTextArray = new string[0];
                Text = string.Empty;
                _inClear = false;
            }
            else
            {
                var selectedValues = new List<string>();
                foreach (CheckComboBoxItem item in ItemsSource)
                    if (item.IsChecked) selectedValues.Add(item.Text);

                SelectedTextArray = selectedValues.ToArray();
                Text = string.Join(", ", selectedValues.ToArray());
            }
        }
    }
}
