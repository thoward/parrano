using System;

namespace Parrano.Api
{
    /// <summary>
    /// A class that contains the information fields set in the header of the Postscript document.
    /// </summary>
    public class InfoFields
    {
        #region Static Members
        /// <summary>
        /// A list of valid field names for the PS_set_info API call.
        /// </summary>
        private static readonly string[]
            validFields =
                {
                    "Keywords",
                    "Subject", 
                    "Title",
                    "Creator",
                    "Author",
                    "BoundingBox",
                    "Orientation"
                };

        /// <summary>
        /// Determines whether the specified name is an allowed field name for the PS_set_info Api call.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if the specified name is a valid field name; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidFieldName(string name)
        {
            for (int i = 0; i < validFields.Length; i++)
            {
                if (validFields[i].ToLower() == name.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        #endregion 

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InfoFields"/> class.
        /// </summary>
        public InfoFields()
        {
            _keywords = string.Empty;
            //_subject = string.Empty;
            _title = string.Empty;
            _creator = string.Empty;
            _author = string.Empty;
            _boundingBox = string.Empty;
            _orientation = string.Empty;
        }

        #endregion 

        #region Fields

        private string _author;
        private string _boundingBox;
        private string _creator;
        private string _keywords;
        private string _subject;
        private string _orientation;
        private string _title;

        #endregion 

        #region Properties
        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        /// <value>The keywords.</value>
        public string Keywords
        {
            get { return _keywords; }
            set { _keywords = value; }
        }

         //Subject doesn't seem to work, so I took it out. 
        
        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// Gets or sets the creator.
        /// </summary>
        /// <value>The creator.</value>
        public string Creator
        {
            get { return _creator; }
            set { _creator = value; }
        }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        /// <summary>
        /// Gets or sets the bounding box.
        /// </summary>
        /// <value>The bounding box.</value>
        public string BoundingBox
        {
            get { return _boundingBox; }
            set { _boundingBox = value; }
        }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public string Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }

        #endregion 

        #region Methods

        /// <summary>
        /// Updates the Postscript document with the specified field names. If beginpage has already been called once, then this has no effect on the document.
        /// </summary>
        /// <param name="psdoc">The psdoc.</param>
        public void UpdateDocument(IntPtr psdoc)
        {
            if (!string.IsNullOrEmpty(Author))
            {
                PSLib.PS_set_info(psdoc, "Author", Author);
            }

            if (!string.IsNullOrEmpty(BoundingBox))
            {
                PSLib.PS_set_info(psdoc, "BoundingBox", BoundingBox);
            }

            if (!string.IsNullOrEmpty(Creator))
            {
                PSLib.PS_set_info(psdoc, "Creator", Creator);
            }

            if (!string.IsNullOrEmpty(Keywords))
            {
                PSLib.PS_set_info(psdoc, "Keywords", Keywords);
            }

            if (!string.IsNullOrEmpty(Orientation))
            {
                PSLib.PS_set_info(psdoc, "Orientation", Orientation);
            }

            if (!string.IsNullOrEmpty(this.Subject))
            {
                PSLib.PS_set_info(psdoc, "Subject", this.Subject);
            }

            if (!string.IsNullOrEmpty(Title))
            {
                PSLib.PS_set_info(psdoc, "Title", Title);
            }
        }

        #endregion
    }
}