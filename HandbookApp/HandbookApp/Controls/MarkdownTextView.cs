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
using System.IO;
using Xamarin.Forms;
using CommonMark;
using CommonMark.Syntax;


namespace HandbookApp.Controls
{
    public class MarkdownTextView : View
    {
        public MarkdownTextView()
        {
        }

        public static readonly BindableProperty MarkdownProperty =
            BindableProperty.Create(
                propertyName: "Markdown",
                returnType: typeof(Block),
                declaringType: typeof(MarkdownTextView),
                defaultValue: default(Block)
            );

        public Block Markdown
        {
            get { return (Block)GetValue(MarkdownProperty); }
            set { SetValue(MarkdownProperty, value); }
        }

        public static readonly BindableProperty HtmlProperty =
            BindableProperty.Create(
                propertyName: "Html",
                returnType: typeof(string),
                declaringType: typeof(MarkdownTextView),
                defaultValue: default(string)
            );

        public string Html
        {
            get { return (string)GetValue(HtmlProperty); }
            set { SetValue(HtmlProperty, value); }
        }

        public string GetHtml()
        {
            if (this.Html != null && this.Html != "")
                return this.Html;

            if (this.Markdown == null)
                return "";

            var document = this.Markdown;
            var target = new StringWriter();

            CommonMarkConverter.ProcessStage3(document, target);

            return target.ToString();
        }

    }
}
