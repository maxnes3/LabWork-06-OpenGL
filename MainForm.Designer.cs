namespace LabWork_06_OpenGL
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            openGLControl = new SharpGL.OpenGLControl();
            ((System.ComponentModel.ISupportInitialize)openGLControl).BeginInit();
            SuspendLayout();
            // 
            // openGLControl
            // 
            openGLControl.AutoSize = true;
            openGLControl.Dock = DockStyle.Fill;
            openGLControl.DrawFPS = false;
            openGLControl.Location = new Point(0, 0);
            openGLControl.Margin = new Padding(4, 3, 4, 3);
            openGLControl.Name = "openGLControl";
            openGLControl.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
            openGLControl.RenderContextType = SharpGL.RenderContextType.DIBSection;
            openGLControl.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
            openGLControl.Size = new Size(1096, 662);
            openGLControl.TabIndex = 0;
            openGLControl.OpenGLDraw += openGLControl_OpenGLDraw;
            openGLControl.Load += MainForm_Load;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1096, 662);
            Controls.Add(openGLControl);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "LabWork_06_OpenGL";
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)openGLControl).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SharpGL.OpenGLControl openGLControl;
    }
}
