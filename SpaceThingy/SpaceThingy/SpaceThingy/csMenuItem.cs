using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceThingy
{
    public class csMenuItem
    {
        public string text;
        public string textafteraction;
        public csEvent.events eventItem;
        public bool wasSelected;

        public string displaytext
        {
            get
            {
                if (wasSelected)
                    return textafteraction;
                else
                    return text;
            }
        }

        public csMenuItem()
        {
            text = "NOT SET";
            textafteraction = "NOT SET";
            eventItem = csEvent.events.None;
            wasSelected = false;
        }

        public void selected()
        {
            csEvent.executeEvent(eventItem);
            wasSelected = true;
        }
    }
}
