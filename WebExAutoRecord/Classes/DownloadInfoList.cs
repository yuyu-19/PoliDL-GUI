using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PoliDLGUI.Classes
{
    public class DownloadInfoList
    {
        public List<DownloadInfo> list = new List<DownloadInfo>();

        internal void Remove(DownloadInfo downloadInfo)
        {
            lock (this)
            {
                try
                {
                    this.list.Remove(downloadInfo);
                }
                catch
                {
                    ;
                }
            }
        }

        public bool Contains(DownloadInfo downloadInfo)
        {
            lock (this)
            {
                try
                {
                    foreach (var x in this.list)
                    {
                        if (x == downloadInfo)
                            return true;
                    }
                }
                catch
                {
                    ;
                }
            }

            return false;
        }

        internal void Add(DownloadInfo downloadInfo)
        {
            if (downloadInfo == null)
                return;

            lock (this)
            {
                bool contained = this.Contains(downloadInfo);
                if (!contained)
                {
                    this.list.Add(downloadInfo);
                }
            }
        }

        internal void LoadPanelResults(ref Panel panel1)
        {
            const int HEIGHT = 20;
            int WIDTH_LABEL = panel1.Width / 10 * 7;
            int WIDTH_BUTTON = panel1.Width / 10 * 3;
            _ = new Label()
            {
                Location = new System.Drawing.Point(x: 0, y: 0),
                Size = new System.Drawing.Size(WIDTH_LABEL, HEIGHT),
                Parent = panel1,
                Text = "URL"
            };

            for (int i = 0; i < this.list.Count; i++)
            {
                var p = this.list[i];
                _ = new Label()
                {
                    Location = new System.Drawing.Point(x: 0, y: (i + 1) * HEIGHT),
                    Size = new System.Drawing.Size(WIDTH_LABEL, HEIGHT),
                    Parent = panel1,
                    Text = p.uri.ToString()
                };

                Button b = new Button()
                {
                    Location = new System.Drawing.Point(WIDTH_LABEL, (i + 1) * HEIGHT),
                    Size = new System.Drawing.Size(WIDTH_BUTTON, HEIGHT),
                    Parent = panel1,
                    Text = "More info"
                };

                b.Click += (sender, args) => p.ClickedMoreInfo();
            }
        }

        internal int GetCount()
        {
            return this.list.Count;
        }

        internal DownloadInfo Find(Predicate<DownloadInfo> pred)
        {
            lock (this)
            {
                try
                {
                    return this.list.Find(pred);
                }
                catch
                {
                    ;
                }
            }

            return null;
        }

        public IEnumerable<object> Select(Func<DownloadInfo, object> selector)
        {
            lock (this)
            {
                try
                {
                    IEnumerable<object> x = this.list.Select(selector);
                    return x;
                }
                catch
                {
                    ;
                }
            }

            return null;
        }

        internal void KillAll()
        {
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    try
                    {
                        DownloadInfo x = list[i];
                        x.process.Kill();
                        x.process.Dispose();
                        list.Remove(x);
                        i--;
                    }
                    catch
                    {
                        ;
                    }
                }
            }
            catch
            {
                ;
            }
        }

        internal List<string> GetURIs()
        {
            lock (this)
            {
                try
                {
                    return list.Select(x => x.uri.ToString()).ToList();
                }
                catch
                {
                    ;
                }
            }

            return null;
        }

        internal DownloadInfo GetAndRemoveFirst()
        {
            lock (this)
            {
                if (this.list.Count > 0)
                {
                    var p = this.list[0];
                    this.list.RemoveAt(0);
                    return p;
                }
            }

            return null;
        }
    }
}