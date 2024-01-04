
namespace VORONOI
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.list_box = new System.Windows.Forms.ListBox();
            this.save = new System.Windows.Forms.Button();
            this.read_draw = new System.Windows.Forms.Button();
            this.clear = new System.Windows.Forms.Button();
            this.nextstep = new System.Windows.Forms.Button();
            this.run = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // list_box
            // 
            this.list_box.FormattingEnabled = true;
            this.list_box.ItemHeight = 12;
            this.list_box.Location = new System.Drawing.Point(608, 44);
            this.list_box.Name = "list_box";
            this.list_box.Size = new System.Drawing.Size(201, 604);
            this.list_box.TabIndex = 1;
            // 
            // save
            // 
            this.save.AccessibleDescription = "243";
            this.save.BackColor = System.Drawing.SystemColors.ControlLight;
            this.save.Image = global::VORONOI.Properties.Resources.save;
            this.save.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.save.Location = new System.Drawing.Point(271, -2);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(48, 48);
            this.save.TabIndex = 8;
            this.save.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.save.UseVisualStyleBackColor = false;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // read_draw
            // 
            this.read_draw.AccessibleDescription = "243";
            this.read_draw.BackColor = System.Drawing.SystemColors.ControlLight;
            this.read_draw.Image = global::VORONOI.Properties.Resources.draw;
            this.read_draw.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.read_draw.Location = new System.Drawing.Point(225, -2);
            this.read_draw.Name = "read_draw";
            this.read_draw.Size = new System.Drawing.Size(48, 48);
            this.read_draw.TabIndex = 7;
            this.read_draw.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.read_draw.UseVisualStyleBackColor = false;
            this.read_draw.Click += new System.EventHandler(this.read_draw_Click);
            // 
            // clear
            // 
            this.clear.AccessibleDescription = "243";
            this.clear.BackColor = System.Drawing.SystemColors.ControlLight;
            this.clear.Image = global::VORONOI.Properties.Resources.eraser;
            this.clear.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.clear.Location = new System.Drawing.Point(180, -2);
            this.clear.Name = "clear";
            this.clear.Size = new System.Drawing.Size(48, 48);
            this.clear.TabIndex = 6;
            this.clear.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.clear.UseVisualStyleBackColor = false;
            this.clear.Click += new System.EventHandler(this.clear_Click);
            // 
            // nextstep
            // 
            this.nextstep.AccessibleDescription = "243";
            this.nextstep.Image = ((System.Drawing.Image)(resources.GetObject("nextstep.Image")));
            this.nextstep.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.nextstep.Location = new System.Drawing.Point(135, -2);
            this.nextstep.Name = "nextstep";
            this.nextstep.Size = new System.Drawing.Size(48, 48);
            this.nextstep.TabIndex = 5;
            this.nextstep.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.nextstep.UseVisualStyleBackColor = true;
            this.nextstep.Click += new System.EventHandler(this.nextstep_Click);
            // 
            // run
            // 
            this.run.AccessibleDescription = "243";
            this.run.Image = ((System.Drawing.Image)(resources.GetObject("run.Image")));
            this.run.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.run.Location = new System.Drawing.Point(91, -2);
            this.run.Name = "run";
            this.run.Size = new System.Drawing.Size(48, 48);
            this.run.TabIndex = 4;
            this.run.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.run.UseVisualStyleBackColor = true;
            this.run.Click += new System.EventHandler(this.run_Click);
            // 
            // button2
            // 
            this.button2.AccessibleDescription = "243";
            this.button2.Image = global::VORONOI.Properties.Resources._3847912_arrow_next_right_icon_removebg_preview;
            this.button2.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button2.Location = new System.Drawing.Point(47, -2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(48, 48);
            this.button2.TabIndex = 3;
            this.button2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.nextdata);
            // 
            // button1
            // 
            this.button1.AccessibleDescription = "243";
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button1.Location = new System.Drawing.Point(2, -2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(48, 48);
            this.button1.TabIndex = 2;
            this.button1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.openfolder);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(2, 44);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(600, 600);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            // 
            // button3
            // 
            this.button3.AccessibleDescription = "243";
            this.button3.BackColor = System.Drawing.SystemColors.ControlLight;
            this.button3.Image = global::VORONOI.Properties.Resources.save;
            this.button3.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button3.Location = new System.Drawing.Point(347, -2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(48, 48);
            this.button3.TabIndex = 9;
            this.button3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 645);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.save);
            this.Controls.Add(this.read_draw);
            this.Controls.Add(this.clear);
            this.Controls.Add(this.nextstep);
            this.Controls.Add(this.run);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.list_box);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ListBox list_box;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button run;
        private System.Windows.Forms.Button nextstep;
        private System.Windows.Forms.Button clear;
        private System.Windows.Forms.Button read_draw;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.Button button3;
    }
}

