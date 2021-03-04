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

        internal void Add(DownloadInfo downloadInfo)
        {
            if (downloadInfo == null)
                return;

            lock (this)
            {
                foreach (var x in this.list)
                {
                    if (x == downloadInfo)
                        return;
                }

                this.list.Add(downloadInfo);
            }
        }

        internal void LoadPanelResults(ref Panel panel1)
        {
            const int HEIGHT = 20;
            int WIDTH_LABEL = panel1.Width;
            for (int i=0; i<this.list.Count; i++)
            {
                Label label = new Label() { 
                    Location = new System.Drawing.Point(x: 0,y: i*HEIGHT), 
                    Size = new System.Drawing.Size(WIDTH_LABEL, HEIGHT ), 
                    Parent = panel1, 
                    Text = this.list[i].uri.ToString()
                };
            }
        }

        internal int? GetCount()
        {
            try
            {
                return this.list.Count;
            }
            catch
            {
                ;
            }

            return null;
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
                    DownloadInfo x = list[i];
                    try
                    {
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