namespace Macro.ImageViewer.Web.Client.Silverlight.Resources
{
    public sealed class ImageViewerResources
    {
        private static readonly Labels _labels = new Labels();
   
          private static readonly SR _sr = new SR();
   
        /// <summary>
        /// Gets the <see cref="Labels"/>.
        /// </summary>
        public Labels Labels
        {
            get { return _labels; }
        }
               
        /// <summary>
        /// Gets the <see cref="SR"/>.
        /// </summary>
        public SR SR
        {
            get { return _sr; }
        }     
    }
}
