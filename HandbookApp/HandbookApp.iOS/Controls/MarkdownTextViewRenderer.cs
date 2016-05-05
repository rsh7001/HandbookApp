//
//  Copyright 2016  R. Stanley Hum <r.stanley.hum@gmail.com>
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using Foundation;
using UIKit;

using HandbookApp.Controls;
using HandbookApp.iOS.Controls;


[assembly: ExportRenderer(typeof(MarkdownTextView), typeof(MarkdownTextViewRenderer))]
namespace HandbookApp.iOS.Controls
{
    public class MarkdownTextViewRenderer : ViewRenderer<MarkdownTextView, UITextView>
    {
        MarkdownTextView _view;

        public MarkdownTextViewRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<MarkdownTextView> e)
        {
            base.OnElementChanged(e);

            _view = Element;

            base.SetNativeControl(new UITextView());

            var attr = new NSAttributedStringDocumentAttributes {
                DocumentType = NSDocumentType.HTML
            };
            var error = new NSError();

            var html = _view.GetHtml();

            if (Control != null)
            {
                Control.Editable = false;
                Control.AttributedText = new NSAttributedString(html, attr, ref error);
                Control.Selectable = true;
                Control.DataDetectorTypes = UIDataDetectorType.All;
                Control.ScrollEnabled = false;
            }
        }

    }
}
