using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Dragoon_Modifier {
    public class UniformTabPanel : UniformGrid {
        public UniformTabPanel() {
            IsItemsHost = true;
            Rows = 1;
            HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        protected override Size MeasureOverride(Size constraint) {
            var totalMaxWidth = Children.OfType<TabItem>().Sum(tab => tab.MaxWidth);
            if (!double.IsInfinity(totalMaxWidth))
                HorizontalAlignment = constraint.Width > totalMaxWidth
                    ? HorizontalAlignment.Left
                    : HorizontalAlignment.Stretch;

            return base.MeasureOverride(constraint);
        }
    }
}