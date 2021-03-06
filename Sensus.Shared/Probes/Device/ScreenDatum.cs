// Copyright 2014 The Rector & Visitors of the University of Virginia
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Sensus.Probes.User.Scripts.ProbeTriggerProperties;
using System;

namespace Sensus.Probes.Device
{
    public class ScreenDatum : Datum
    {
        private bool _on;

        [BooleanProbeTriggerProperty]
        public bool On
        {
            get { return _on; }
            set { _on = value; }
        }

        public override string DisplayDetail
        {
            get { return _on ? "On" : "Off"; }
        }

        /// <summary>
        /// Gets the string placeholder value, which is whether the screen is on/off.
        /// </summary>
        /// <value>The string placeholder value.</value>
        public override object StringPlaceholderValue
        {
            get
            {
                return _on;
            }
        }

        /// <summary>
        /// For JSON deserialization.
        /// </summary>
        private ScreenDatum() { }

        public ScreenDatum(DateTimeOffset timestamp, bool on)
            : base(timestamp)
        {
            _on = on;
        }

        public override string ToString()
        {
            return base.ToString() + Environment.NewLine +
                   "On:  " + _on;
        }
    }
}
