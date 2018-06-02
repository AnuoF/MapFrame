using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.GlobeCore;
using GlobleSituation.Interface;

namespace GlobleSituation.Business
{
    public class BookmarkManager : IBookmarkManager
    {

        //类成员  
        private IArray m_BookmarkArray = null;
        private string m_bookmarkName = "";
        private IGlobe globe = null;

        public BookmarkManager(ESRI.ArcGIS.Controls.AxGlobeControl axGlobeControl1)
        {
            globe = axGlobeControl1.Globe;
            ISceneBookmarks2 sceneBookmarks = globe as ISceneBookmarks2;
            m_BookmarkArray = sceneBookmarks.Bookmarks;
            for (int i = 0; i < sceneBookmarks.BookmarkCount; i++)
            {
                IBookmark3D pBookmark = new Bookmark3DClass();
                pBookmark = m_BookmarkArray.get_Element(i) as IBookmark3D;
            }
        }

        public void CraeteBookmark(string markName)
        {
            ISceneBookmarks pBookmarks = globe.GlobeDisplay.Scene as ISceneBookmarks;
            IBookmark3D pBookmark3D = new Bookmark3DClass();
            pBookmark3D.Name = markName;
            pBookmark3D.Capture(globe.GlobeDisplay.ActiveViewer.Camera);
            pBookmarks.AddBookmark(pBookmark3D);
            m_bookmarkName = markName;
        }

        public void DeleteBookmark(string markName)
        {
            ISceneBookmarks2 sceneBookmarks = globe as ISceneBookmarks2;
            IBookmark3D bookmark3D = null;
            sceneBookmarks.FindBookmark(markName, out bookmark3D);
            if (bookmark3D != null)
            {
                sceneBookmarks.RemoveBookmark(bookmark3D);
            }
        }

        public void Move2Bookmark(string markName)
        {
            ISceneBookmarks2 sceneBookmarks = globe as ISceneBookmarks2;
            IBookmark3D bookmark3D = null;
            sceneBookmarks.FindBookmark(markName, out bookmark3D);
            if (bookmark3D != null)
            {
                bookmark3D.Apply(globe.GlobeDisplay.ActiveViewer, true, 0);
                globe.GlobeDisplay.RefreshViewers();
            }
        }
    }
}
