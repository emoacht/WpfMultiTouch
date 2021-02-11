using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace WpfMultiTouch
{
	public class MultiTouchBehavior : Behavior<ButtonBase>
	{
		public object TargetObject
		{
			get { return (object)GetValue(TargetObjectProperty); }
			set { SetValue(TargetObjectProperty, value); }
		}
		public static readonly DependencyProperty TargetObjectProperty =
			DependencyProperty.Register(
				"TargetObject",
				typeof(object),
				typeof(MultiTouchBehavior),
				new PropertyMetadata(
					null,
					(d, e) => ((MultiTouchBehavior)d).SetMethods()));

		public string SingleTouchClickMethodName { get; set; }
		public string MultiTouchClickMethodName { get; set; }

		private MethodInfo _singleTouchClickMethod;
		private MethodInfo _multiTouchClickMethod;

		private void SetMethods()
		{
			if (TargetObject is null)
				return;

			var targetType = TargetObject.GetType();

			if (!string.IsNullOrEmpty(SingleTouchClickMethodName))
				_singleTouchClickMethod = targetType.GetMethod(SingleTouchClickMethodName, Type.EmptyTypes);

			if (!string.IsNullOrEmpty(MultiTouchClickMethodName))
				_multiTouchClickMethod = targetType.GetMethod(MultiTouchClickMethodName, Type.EmptyTypes);
		}

		protected override void OnAttached()
		{
			base.OnAttached();

			this.AssociatedObject.PreviewTouchDown += OnPreviewTouchDown;
			this.AssociatedObject.PreviewTouchUp += OnPreviewTouchUp;
			this.AssociatedObject.Click += OnClick;
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			this.AssociatedObject.PreviewTouchDown -= OnPreviewTouchDown;
			this.AssociatedObject.PreviewTouchUp -= OnPreviewTouchUp;
			this.AssociatedObject.Click -= OnClick;
		}

		private readonly HashSet<int> _touchDeviceIds = new HashSet<int>();

		private void OnPreviewTouchDown(object sender, TouchEventArgs e)
		{
			_touchDeviceIds.Add(e.TouchDevice.Id);
		}

		private void OnPreviewTouchUp(object sender, TouchEventArgs e)
		{
			Task.Run(async () =>
			{
				await Task.Delay(TimeSpan.FromSeconds(1));
				_touchDeviceIds.Clear();
			});
		}

		private void OnClick(object sender, RoutedEventArgs e)
		{
			var isMultiTouch = (_touchDeviceIds.Count > 1);
			_touchDeviceIds.Clear();

			if (!isMultiTouch)
			{
				_singleTouchClickMethod?.Invoke(TargetObject, null);
			}
			else
			{
				_multiTouchClickMethod?.Invoke(TargetObject, null);
			}
		}
	}
}