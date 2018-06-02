
namespace GlobleSituation.Interface
{
    public interface IBookmarkManager
    {
        /// <summary>
        /// 创建书签
        /// </summary>
        /// <param name="markName"></param>
        void CraeteBookmark(string markName);

        /// <summary>
        /// 缩放至书签
        /// </summary>
        /// <param name="markName"></param>
        void Move2Bookmark(string markName);

        /// <summary>
        /// 删除书签
        /// </summary>
        /// <param name="markName"></param>
        void DeleteBookmark(string markName);

    }
}
