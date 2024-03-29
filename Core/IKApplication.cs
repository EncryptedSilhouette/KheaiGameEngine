﻿using System.Collections;

namespace KheaiGameEngine.Core
{
    ///<summary>
    ///A basic interface for a KApplication.
    ///</summary>
    public interface IKApplication
    {
        public string AppName { get; set; }
        public IKEngine Engine { get; set; }

        public void Load();

        public void Init();

        public void Start();
    }
}
