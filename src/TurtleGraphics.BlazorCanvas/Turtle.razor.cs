using System;
using Microsoft.AspNetCore.Components;
using Blazor.Extensions;
using Blazor.Extensions.Canvas;
using Blazor.Extensions.Canvas.Canvas2D;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using System.Drawing;
using System.Collections.Generic;

namespace TurtleGraphics.BlazorCanvas
{
    public partial class Turtle : IDisposable
    {        
        private Canvas2DContext? _context;
        private BECanvasComponent? _canvas;
        private TurtleHead _turtle = new() { Width = 35, Height = 35 };

        [Parameter]
        public long Height { get; set; }

        [Parameter]
        public long Width { get; set; }

        [Inject]
        public TurtleJsInterop? TurtleJsInterop { get; set; }

        public bool ShowTurtle
        {
            get => _turtle.ShowTurtle;
            set => _turtle = _turtle with { ShowTurtle = value };
        }

        public bool PenVisible
        {
            get => _turtle.PenVisible;
            set => _turtle = _turtle with { PenVisible = value };
        }

        public Color PenColor
        {
            get => _turtle.PenColor;
            set => _turtle = _turtle with { PenColor = value };
        }

        public float PenSize
        {
            get => _turtle.PenSize;
            set => _turtle = _turtle with { PenSize = value };
        }

        public int Delay
        {
            get => _turtle.Delay;
            set => _turtle = _turtle with { Delay = value };
        }

        private string RotateStyle => $"rotate({_turtle.Angle}deg)";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            await TurtleJsInterop.Init(this);            
            _context = await _canvas.CreateCanvas2DAsync();            
        }

        public async ValueTask Step()
        {            
            await DrawTurtle();

            StateHasChanged();
        }

        public async ValueTask OnResize(int width, int height)
        {
            Width = width;
            Height = height;

            await Reset();
        }

        public async Task Reset()
        {
            // Clear the surface area
            await _context.ClearRectAsync(0, 0, Width, Height);

            _turtle = _turtle with
            {
                // Initialize the pen size and color
                PenSize = TurtleHead.DefaultPenSize,
                PenColor = TurtleHead.DefaultColor,

                // Initialize the turtle position and otehr settings
                X = 0,
                Y = 0,
                Angle = 0,
                PenVisible = true,
                ShowTurtle = true,

                // Intentionally preserve the "Delay" settings
                //Delay = 0
            };

            StateHasChanged();
        }

        public async Task Forward(float distance = 10)
        {
            var angleRadians = _turtle.Angle * Math.PI / 180;
            var newX = _turtle.X + (float)(distance * Math.Sin(angleRadians));
            var newY = _turtle.Y + (float)(distance * Math.Cos(angleRadians));
            await MoveTo(newX, newY);
        }

        public async Task Backward(float distance = 10)
        {
            await Forward(-distance);
        }

        public async Task MoveTo(float newX, float newY)
        {
            var fromX = Width / 2 + _turtle.X;
            var fromY = Height / 2 - _turtle.Y;
            _turtle = _turtle with { X = newX, Y = newY };
            if (_turtle.PenVisible)
            {
                var toX = Width / 2 + _turtle.X;
                var toY = Height / 2 - _turtle.Y;

                await _context.SetLineCapAsync(LineCap.Round);
                await _context.BeginPathAsync();
                await _context.SetLineWidthAsync(_turtle.PenSize);
                await _context.SetStrokeStyleAsync(ColorTranslator.ToHtml(_turtle.PenColor));
                await _context.MoveToAsync(fromX, fromY);
                await _context.LineToAsync(toX, toY);
                await _context.StrokeAsync();
                await _context.ClosePathAsync();
            }

            await DrawTurtle();
        }

        public async Task Rotate(float angleDelta)
        {
            _turtle = _turtle with { Angle = _turtle.Angle + angleDelta };
            await DrawTurtle();
        }

        public async Task RotateTo(float newAngle)
        {
            _turtle = _turtle with { Angle = newAngle };
            await DrawTurtle();
        }

        public void PenUp()
        {
            _turtle = _turtle with { PenVisible = false };
        }

        public void PenDown()
        {
            _turtle = _turtle with { PenVisible = true };
        }

        private async Task DrawTurtle()
        {
            await Task.Delay(_turtle.Delay);

            var turtleX = 1 + Width / 2 + _turtle.X - _turtle.Width / 2;
            var turtleY = 1 + Height / 2 - _turtle.Y - _turtle.Height / 2;

            _turtle = _turtle with { Left = (int)Math.Round(turtleX) };
            _turtle = _turtle with { Top = (int)Math.Round(turtleY) };
        }

        #region IDisposable
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _context?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Turtle()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion // IDisposable
    }
}
