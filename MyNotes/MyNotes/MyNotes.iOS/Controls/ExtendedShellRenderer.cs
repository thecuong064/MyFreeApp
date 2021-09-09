using CoreGraphics;
using MyNotes.iOS.Controls;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Shell), typeof(ExtendedShellRenderer))]
namespace MyNotes.iOS.Controls
{
    public class ExtendedShellRenderer : ShellRenderer
    {
        protected override IShellSectionRenderer CreateShellSectionRenderer(ShellSection shellSection)
        {
            return new CustomShellSectionRenderer(this);
        }

        private class CustomShellSectionRenderer : ShellSectionRenderer
        {
            public CustomShellSectionRenderer(IShellContext context) : base(context)
            {
            }

            protected override void UpdateTabBarItem()
            {
                base.UpdateTabBarItem();
                //TODO: Calculate the size according the screen.
                //According to Apple:
                //@1x: 48x32
                //@2x: 96x64
                //@3x: 144x96

                var width = 48;
                var height = 32;
                if (UIScreen.MainScreen.Scale == 2.0) //@2x iPhone 6 7 8 
                {
                    width *= 2;
                    height *= 2;
                }
                else if (UIScreen.MainScreen.Scale == 3.0) //@3x iPhone 6p 7p 8p...
                {
                    width *= 3;
                    height *= 3;
                }

                TabBarItem.Image = ResizeImage(TabBarItem.Image, width, height);
            }

            private UIImage ResizeImage(UIImage source, float width, float height)
            {
                if (source == null)
                    return null;

                //Calculate the resize factor
                CGSize sourceSize = source.Size;
                double resizeFactor = Math.Min(width / sourceSize.Width, height / sourceSize.Height);

                //If the resize factor is greater than 1 (i.e. the Width and Height are bigger than source's size), then DON'T rescale
                if (resizeFactor > 1)
                    return source;

                //Calculate the new size
                double resizeWidth = sourceSize.Width * resizeFactor;
                double resizeHeight = sourceSize.Height * resizeFactor;

                //NOTE: DON'T use UIGraphics.BeginImageContext, otherwise the final result will lose some quality.
                UIGraphics.BeginImageContextWithOptions(new CGSize(resizeWidth, resizeHeight), false, 0.0f);

                //Redraw the image in the new size
                source.Draw(new CGRect(0, 0, resizeWidth, resizeHeight));

                //Save the new image for later
                UIImage scaledImage = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();

                return scaledImage;
            }
        }
    }
}