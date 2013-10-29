using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Base
{
    public interface IControllable
    {
        void TakeControl(IController controller);
        void ReleaseControl(IController controller);
        void ControlUpdate(IController controller);
    }
}
