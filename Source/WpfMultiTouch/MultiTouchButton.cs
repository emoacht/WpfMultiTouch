using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfMultiTouch
{
	public class MultiTouchButton : Button
	{
		public bool IsMultiTouch => (_touchDeviceIds.Count > 1);

		private readonly HashSet<int> _touchDeviceIds = new HashSet<int>();

		protected override void OnPreviewTouchDown(TouchEventArgs e)
		{
			base.OnPreviewTouchDown(e);

			_touchDeviceIds.Add(e.TouchDevice.Id);
		}

		protected override void OnPreviewTouchUp(TouchEventArgs e)
		{
			base.OnPreviewTouchUp(e);

			Task.Run(async () =>
			{
				await Task.Delay(TimeSpan.FromSeconds(1));
				_touchDeviceIds.Clear();
			});
		}

		protected override void OnClick()
		{
			base.OnClick();

			Trace.WriteLine($"{(IsMultiTouch ? "Multi" : "Single")}");

			_touchDeviceIds.Clear();
		}
	}
}