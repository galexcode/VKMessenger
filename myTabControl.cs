using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace IMV
{

    public class myTabControl : TabControl
    {
        #region	Конструктор

        int _Radius;

        public myTabControl()
        {

            this.SetStyle(ControlStyles.UserPaint, true); // рисуем сами
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true); // игнорируем сообщение виндовс о стирании фона
            this.SetStyle(ControlStyles.Opaque, true); // всё рисуем в одном методе, использование встроенного метода для рисования фона отключено, элемент пока непрозрачен
            this.SetStyle(ControlStyles.ResizeRedraw, true); // при изменении размеров перерисовываем

            this._BackBuffer = new Bitmap(this.Width, this.Height); // создаём новое буферное изображение
            this._BackBufferGraphics = Graphics.FromImage(this._BackBuffer); // создаём объект для рисования в буфере
            this._TabBuffer = new Bitmap(this.Width, this.Height); // создаём новое буферное изображение для закладок
            this._TabBufferGraphics = Graphics.FromImage(this._TabBuffer); // создаём объект для рисования в буфере закладок

            this._Radius = 5; // закругление вкладок
            this.Padding = new Point(6, 3); // внутренний отступ
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (this._BackImage != null)
                {
                    this._BackImage.Dispose();
                }
                if (this._BackBufferGraphics != null)
                {
                    this._BackBufferGraphics.Dispose();
                }
                if (this._BackBuffer != null)
                {
                    this._BackBuffer.Dispose();
                }
                if (this._TabBufferGraphics != null)
                {
                    this._TabBufferGraphics.Dispose();
                }
                if (this._TabBuffer != null)
                {
                    this._TabBuffer.Dispose();
                }
            }
        }

        #endregion

        #region Переменные

        private Bitmap _BackImage; // Буферное изображение (холст)
        private Bitmap _BackBuffer; // Буфер, в который записывается всё рисование
        private Graphics _BackBufferGraphics; // Графика для буфера
        private Bitmap _TabBuffer; // Буферное изображение для закладок
        private Graphics _TabBufferGraphics; // Графика для буфера закладок

        private int _oldValue;

        #endregion

        #region	Переопределение методов

        protected override void OnMouseClick(MouseEventArgs e)
        {
            int index = this.SelectedIndex;
            // Если курсор находится над крестиком
            if (this.GetTabCloserRect(index).Contains(this.MousePosition))
            {

                // Удаляем вкладку и связанные с ней элементы

                vars.VARS.Chat.richtbox.Remove((uint)this.SelectedTab.Tag);
                vars.VARS.Chat.textbox.Remove((uint)this.SelectedTab.Tag);
                this.TabPages.RemoveByKey(SelectedTab.Name);
                if (this.TabPages.Count == 0)
                {
                    vars.VARS.Chat.Hide();
                    vars.VARS.Chat.Text = "";
                }
                else
                    this.SelectTab(this.TabPages.Count - 1);

                //this.Invalidate();
            }
            else
            {
                //	Иначе просто обрабатываем клик
                base.OnMouseClick(e);
            }
        }

        protected override void OnResize(EventArgs e)
        {// При изменении размера элемента очищаем наш буфер и создаём новый
            if (this.Width > 0 && this.Height > 0)
            { // зануляем и уничтожаем всё предыдущее
                if (this._BackImage != null)
                {
                    this._BackImage.Dispose();
                    this._BackImage = null;
                }
                if (this._BackBufferGraphics != null)
                {
                    this._BackBufferGraphics.Dispose();
                }
                if (this._BackBuffer != null)
                {
                    this._BackBuffer.Dispose();
                }

                this._BackBuffer = new Bitmap(this.Width, this.Height); // создаём новый буферный холст
                this._BackBufferGraphics = Graphics.FromImage(this._BackBuffer); // создаём для этого холста объект для рисования
                // зануляем и уничтожаем....
                if (this._TabBufferGraphics != null)
                {
                    this._TabBufferGraphics.Dispose();
                }
                if (this._TabBuffer != null)
                {
                    this._TabBuffer.Dispose();
                }

                this._TabBuffer = new Bitmap(this.Width, this.Height); // создаём новый буферный холст для вкладок
                this._TabBufferGraphics = Graphics.FromImage(this._TabBuffer); // создаём для этого холста объект для рисования

                if (this._BackImage != null)
                {
                    this._BackImage.Dispose();
                    this._BackImage = null;
                }

            }
            base.OnResize(e);
        }

        protected override void OnParentBackColorChanged(EventArgs e)
        {
            if (this._BackImage != null)
            {
                this._BackImage.Dispose();
                this._BackImage = null;
            }
            base.OnParentBackColorChanged(e);
        }

        private void OnParentResize(object sender, EventArgs e)
        { // если сменился размер окна, в котором находится tabControl, мы должны перерисовать
            if (this.Visible)
            {
                this.Invalidate(); // говорим, что область недействительна и надо её перерисовать
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (this.Parent != null)
            {
                this.Parent.Resize += this.OnParentResize; // при смене размера вызываем...
            }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (this.Visible)
            {
                this.Invalidate(); // если добавлен новый элемен, перерисовываем
            }
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            if (this.Visible)
            {
                this.Invalidate(); // если добавлен новый элемен, перерисовываем
            }
        }

        private void OnHScroll(ScrollEventArgs e)
        {
            //	перерисовать передвинутые вкладки
            this.Invalidate();

            //	Обрабатываем событие

            if (e.Type == ScrollEventType.EndScroll)
            {
                this._oldValue = e.NewValue; // на каком значении положении остановился скролл
            }
        }

        #endregion

        #region	Рисование

        protected override void OnPaint(PaintEventArgs e)
        {
            //	Если e.ClipRectagnle включает, то
            if (e.ClipRectangle.Equals(this.ClientRectangle))
            {
                this.CustomPaint(e.Graphics);
            }
            else
            {
                //	либо просто добавляем в очередь на перерисовку
                this.Invalidate();
            }
        }

        private void CustomPaint(Graphics screenGraphics)
        {
            if (this.Width > 0 && this.Height > 0)
            {
                if (this._BackImage == null)
                {
                    this._BackImage = new Bitmap(this.Width, this.Height); // Создаём пустое изображение размером элемента управления
                    Graphics backGraphics = Graphics.FromImage(this._BackImage); // Создаём объект рисования для BackImage
                    backGraphics.Clear(Color.Transparent); // Очищаем поверхность и заливаем её прозрачным фоном
                    this.PaintTransparentBackground(backGraphics, this.ClientRectangle); // рисуем "прозрачный" фон для того, чтобы у элемента был фон формы, на которой он находится
                }

                this._BackBufferGraphics.Clear(Color.Transparent); // очищаем поверхность и заливаем её прозрачным фоном
                this._BackBufferGraphics.DrawImageUnscaled(this._BackImage, 0, 0); // рисуем изображение "фона", отрисованного ранее в буфер

                this._TabBufferGraphics.Clear(Color.Transparent); // очищаем поверхность и заливаем её прозрачным фоном

                if (this.TabCount > 0)
                {

                    this._TabBufferGraphics.Clip = new Region(new RectangleF(this.ClientRectangle.X + 3, this.ClientRectangle.Y, this.ClientRectangle.Width - 6, this.ClientRectangle.Height));

                    //	Отрисовываем вкладки справа на лево

                    for (int index = this.TabCount - 1; index >= 0; index--)
                    {
                        if (index != this.SelectedIndex) // выбранную страничку рисуем вне цикла
                        {
                            this.DrawTabPage(index, this._TabBufferGraphics); // вызываем метод рисования странички с закладкой
                        }
                    }

                    //	Выбранную страничку рисуем последней
                    if (this.SelectedIndex > -1)
                    {
                        this.DrawTabPage(this.SelectedIndex, this._TabBufferGraphics);
                    }
                }
                this._TabBufferGraphics.Flush(); // принудительно выполняем все отрисовки и не ждём окончания (надо, чтобы в очереди не стояло)

                this._BackBufferGraphics.DrawImage(this._TabBuffer,
                                                   new Rectangle(0, 0, this._TabBuffer.Width, this._TabBuffer.Height),
                                                   0, 0, this._TabBuffer.Width, this._TabBuffer.Height, GraphicsUnit.Pixel); // попиксельно рисуем буферезированное изображение страничек

                this._BackBufferGraphics.Flush(); // принудительно выполняем все отрисовки и не ждём окончания (надо, чтобы в очереди на отрисовку не стояло)

                //	Сейчас рисуем весь tabControl на экран (фон, вкладки, границы)
                screenGraphics.DrawImageUnscaled(this._BackBuffer, 0, 0);

            }
        }

        private void PaintTransparentBackground(Graphics graphics, Rectangle clipRect)
        {
            graphics.FillRectangle(Brushes.Black, clipRect); // заполняем фон буферного рисунка чёрным
            if ((this.Parent != null))
            {

                //	Смещаем левую верхнюю гранизу для рисования
                clipRect.Offset(this.Location);

                //	Сохраняем текущее состояние
                GraphicsState state = graphics.Save();

                //	Устанавливаем необходимые значения для рисования окна
                graphics.TranslateTransform((float)-this.Location.X, (float)-this.Location.Y); // изменяем координаты для рисования (левый верхний угол)
                graphics.SmoothingMode = SmoothingMode.HighSpeed; // отключаем сгляживание, чтоб быстрее рисовалось

                //	Создаём новый объект аргументов для рисования на родительской форме
                PaintEventArgs e = new PaintEventArgs(graphics, clipRect);
                try
                { // пытаемся нарисовать..
                    this.InvokePaintBackground(this.Parent, e); //.. фон формы
                    this.InvokePaint(this.Parent, e); // саму форму
                }
                finally
                {
                    //	после рисования родительской формы восстанавливаем все параметры
                    graphics.Restore(state); // восстанавливаем состояние объекта
                    clipRect.Offset(-this.Location.X, -this.Location.Y); // убираем смещение
                }
            }
        }

        private void DrawTabPage(int index, Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.HighSpeed; // не вкллючаем сглаживание, чтобы быстрее работало

            //	Получаем границы вкладки
            using (GraphicsPath tabPageBorderPath = this.GetTabPageBorder(index))
            {

                //	Заполняем полученную фигуру
                using (Brush fillBrush = this.GetPageBackgroundBrush(index))
                {
                    graphics.FillPath(fillBrush, tabPageBorderPath);
                }
                //	Рисуем вкладку
                this.PaintTab(index, graphics);

                //	Рисуем изображение
                this.DrawTabImage(index, graphics);

                //	"Пишем" текст
                this.DrawTabText(index, graphics);

                //	Рисуем границы
                this.DrawTabBorder(tabPageBorderPath, index, graphics);

            }
        }

        private void PaintTab(int index, Graphics graphics)
        {
            using (GraphicsPath tabpath = this.GetTabBorder(index)) // Получаем границы для рисования в виде фигуры
            {
                using (Brush fillBrush = this.GetTabBackgroundBrush(index))
                {
                    //	Рисуем фон
                    graphics.FillPath(fillBrush, tabpath);

                    //	Рисуем крестик
                    this.DrawTabCloser(index, graphics);
                }
            }
        }       

        private Brush GetTabBackgroundBrush(int index)
        {
            LinearGradientBrush fillBrush = null;

            //	Цвета для градиента невыбранной вкладки
            Color dark = Color.FromArgb(207, 207, 207);
            Color light = Color.FromArgb(242, 242, 242);

            if (this.SelectedIndex == index) // цвета для выбранной вкладки
            {
                dark = SystemColors.ControlLight;
                light = SystemColors.Window;
            }

            Rectangle tabBounds = this.GetTabRect(index); // получаем размер закладки для правильного отображения градиента
            fillBrush = new LinearGradientBrush(tabBounds, light, dark, LinearGradientMode.Vertical); // создаём кисть с вертикальным градиентом

            return fillBrush;
        }

        private void DrawTabCloser(int index, Graphics graphics)
        {
            Rectangle closerRect = this.GetTabCloserRect(index); // Получаем прямоугольник для рисования
            graphics.SmoothingMode = SmoothingMode.AntiAlias; // Устанавливаем сглаживание линий
            using (GraphicsPath closerPath = GetCloserPath(closerRect)) //Получаем фигуру для рисования
            {
                if (closerRect.Contains(this.MousePosition)) // Если прямоугольник содержит координаты мыши, то
                {
                    using (Pen closerPen = new Pen(Color.Red))
                    {
                        graphics.DrawPath(closerPen, closerPath); // Рисуем крестик красным
                    }
                }
                else // иначе
                {
                    using (Pen closerPen = new Pen(Color.Black))
                    {
                        graphics.DrawPath(closerPen, closerPath); // Рисуем крестик чёрным
                    }
                }

            }
        }

        private Brush GetPageBackgroundBrush(int index)
        {// получаем кисть для закрашивания страницы
            Color light = Color.FromArgb(207, 207, 207); // кисть по умолчанию

            if (this.SelectedIndex == index) // если мы отрисовываем выбранную страницу, то цвет другой
            {
                light = SystemColors.ControlLight;
            }

            return new SolidBrush(light);
        }

        private void DrawTabBorder(GraphicsPath path, int index, Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.HighQuality; // включаем сглаживание
            Color borderColor; // цвет границ закладки
            if (index == this.SelectedIndex) // если рисуем выбранную вкладку, то цвет светлее
            {
                borderColor = Color.LightGray;
            }
            else // иначе темнее
            {
                borderColor = Color.DarkGray;
            }

            using (Pen borderPen = new Pen(borderColor))
            {
                graphics.DrawPath(borderPen, path); // рисуем границу закладки
            }
        }

        private void DrawTabText(int index, Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.HighQuality; // Устанавливаем сглаживание текста
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit; // Устанавливаем самое лучшее качество шрифтов
            Rectangle tabBounds = this.GetTabTextRect(index); // Получаем прямоугольник для рисования текста
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center; // по вертикали на закладке текс будет по центру
            sf.LineAlignment = StringAlignment.Center; // по горисонтали тоже по центру

            using (Brush textBrush = new SolidBrush(Color.FromArgb(255, 0, 0, 0)))
            {
                graphics.DrawString(this.TabPages[index].Text, this.Font, textBrush, tabBounds, sf); // рисуем строку
            }
        }

        private void DrawTabImage(int index, Graphics graphics)
        {
            Image tabImage = null;
            try
            {
                if (vars.VARS.Contact.ContainsKey(Convert.ToUInt32(this.SelectedTab.Tag))) // Если в списке есть контакт с таким номером, то
                {
                    if (vars.VARS.Contact[Convert.ToUInt32(this.TabPages[index].Tag)].online) // Если он онлайн
                        tabImage = Properties.Resources.greenBall;
                    else // Иначе
                        tabImage = Properties.Resources.redBall;
                }
                else // Иначе для пользователя неизвестен его статус, так как не авторизован (не добавлен в друзья)
                    tabImage = Properties.Resources.neverball.ToBitmap();

                Rectangle imageRect = this.GetTabImageRect(index); // получаем прямоугольник для рисования
                graphics.DrawImage(tabImage, imageRect); // рисуем изображение
            }
            catch
            {
                //Ошибка при рисовании, может возникнуть при частой перерисовке, не зависит ни от чего
            }
        }

        #endregion

        #region Получение координат и границ

        private GraphicsPath GetTabBorder(int index)
        {

            GraphicsPath path = new GraphicsPath(); // новая фигура (последовательность соединенных линий и кривых)
            Rectangle tabBounds = this.GetTabRect(index); // прямоугольник закладки

            this.AddTabBorder(path, tabBounds); // собираем новую фигуру для вкладки (прямоугольник с закругленными краями)

            path.CloseFigure(); // соединяем точку начала и точку конца и заканчиваем работтать с фигурой
            return path;
        }

        private GraphicsPath GetCloserPath(Rectangle closerRect)
        {
            GraphicsPath closerPath = new GraphicsPath(); // новая фигура для крустика
            closerPath.AddLine(closerRect.X, closerRect.Y, closerRect.Right, closerRect.Bottom); // линия с левого верхнего в нижний правый угол
            closerPath.CloseFigure();
            closerPath.AddLine(closerRect.Right, closerRect.Y, closerRect.X, closerRect.Bottom); // линия с правого верхнего в нижний левый угол
            closerPath.CloseFigure();
            // получили крестик
            return closerPath;
        }

        private GraphicsPath GetTabPageBorder(int index)
        {
            // создаём страницу с закладкой
            GraphicsPath path = new GraphicsPath();
            Rectangle pageBounds = this.GetPageBounds(index); // прямоугольник для страницы
            Rectangle tabBounds = this.GetTabRect(index); // прямоугольник для закладки
            //tabBounds = new Rectangle(tabBounds.X, tabBounds.Y, tabBounds.Width, tabBounds.Height); // пересоздаём прямоугольник с полученными параметрами
            this.AddTabBorder(path, tabBounds); // создаём границы закладки
            this.AddPageBorder(path, pageBounds, tabBounds); // создаём границы страницы

            path.CloseFigure(); // соединяем концы, завершаем работу с фигурой
            return path;
        }

        private Rectangle GetPageBounds(int index)
        {
            Rectangle pageBounds = this.TabPages[index].Bounds;
            pageBounds.Width += 1;
            pageBounds.Height += 1;
            pageBounds.X -= 1;
            pageBounds.Y -= 1;

            if (pageBounds.Bottom > this.Height - 4)
            {
                pageBounds.Height -= (pageBounds.Bottom - this.Height + 4);
            }
            return pageBounds;
        }

        private Rectangle GetTabTextRect(int index)
        {
            Rectangle textRect = new Rectangle();
            using (GraphicsPath path = this.GetTabBorder(index)) // получаем границы закладки
            {
                RectangleF tabBounds = path.GetBounds(); // получаем информацию о границах (углы, длины, высоты)

                textRect = new Rectangle((int)tabBounds.X, (int)tabBounds.Y, (int)tabBounds.Width, (int)tabBounds.Height); // прямоугольник, где будет рисоваться текст

                textRect.Y += 4; // для того чтобы по центру был текст
                textRect.Height -= 6;

                textRect.X += 6; // Сдвигаем к правому граю (для изображения место)
                textRect.Width -= (textRect.Right - (int)tabBounds.Right); // вычисляем длину прямоугольника
            }
            return textRect;
        }

        private Rectangle GetTabImageRect(int index)
        {
            using (GraphicsPath tabBorderPath = this.GetTabBorder(index))
            {
                Rectangle imageRect = new Rectangle();
                RectangleF rect = tabBorderPath.GetBounds();

                rect.Y += 4;
                rect.Height -= 6;

                imageRect = new Rectangle((int)rect.X, (int)rect.Y + (int)Math.Floor((double)((int)rect.Height - 14) / 2), 14, 14); // вычисляем прямоугольник для изображения (вычисления для того чтобы изображение было посреди вкладки)
                while (!tabBorderPath.IsVisible(imageRect.X, imageRect.Y)) // сдвигаем пока изображение не будет полностью видно
                {
                    imageRect.X += 1;
                }
                return imageRect;
            }
        }

        private Rectangle GetTabCloserRect(int index)
        {
            Rectangle closerRect = new Rectangle();
            using (GraphicsPath path = this.GetTabBorder(index))
            {
                RectangleF rect = path.GetBounds();

                rect.Y += 4;
                rect.Height -= 6;

                closerRect = new Rectangle((int)rect.Right, (int)rect.Y + (int)((rect.Height - 6) / 2) + 1, 6, 6); // вычисляем прямоугольник для крустика (вычисления для того чтобы изображение было посреди вкладки)
                while (!path.IsVisible(closerRect.Right, closerRect.Y) && closerRect.Right > -6) // сдвигаем влево пока крестик не будет полностью видно
                {
                    closerRect.X -= 1;
                }
                closerRect.X -= 2;
            }
            return closerRect;
        }

        private void AddPageBorder(GraphicsPath path, Rectangle pageBounds, Rectangle tabBounds)
        {// создаём границы страницы
            path.AddLine(tabBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Y);
            path.AddLine(pageBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Bottom);
            path.AddLine(pageBounds.Right, pageBounds.Bottom, pageBounds.X, pageBounds.Bottom);
            path.AddLine(pageBounds.X, pageBounds.Bottom, pageBounds.X, pageBounds.Y);
            path.AddLine(pageBounds.X, pageBounds.Y, tabBounds.X, pageBounds.Y);
        }

        private void AddTabBorder(GraphicsPath path, Rectangle tabBounds)
        {// создаём границы закладки с закруглёнными углами
            path.AddLine(tabBounds.X + 1, tabBounds.Bottom, tabBounds.X + 1, tabBounds.Y + this._Radius);
            path.AddArc(tabBounds.X + 1, tabBounds.Y, this._Radius * 2, this._Radius * 2, 180, 90);
            path.AddLine(tabBounds.X + this._Radius, tabBounds.Y, tabBounds.Right - this._Radius, tabBounds.Y);
            path.AddArc(tabBounds.Right - this._Radius * 2, tabBounds.Y, this._Radius * 2, this._Radius * 2, 270, 90);
            path.AddLine(tabBounds.Right, tabBounds.Y + this._Radius, tabBounds.Right, tabBounds.Bottom);
        }

        public new Point MousePosition // Экранируем стандартное свойство
        {
            get
            {
                Point loc = this.PointToClient(Control.MousePosition);
                return loc;
            }
        }

        #endregion
    }
}
