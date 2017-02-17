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

using System;
using Microsoft.Band.Portable;
using Microsoft.Band.Portable.Sensors;
using System.Threading;
using System.Threading.Tasks;

namespace Sensus.Probes.User.MicrosoftBand
{
    public abstract class MicrosoftBandProbe<SensorType, ReadingType> : MicrosoftBandProbeBase
        where SensorType : BandSensorBase<ReadingType>
        where ReadingType : IBandSensorReading
    {
        private SensorType _sensor;

        protected SensorType Sensor
        {
            get
            {
                return _sensor;
            }
        }

        protected override void Configure(BandClient bandClient)
        {
            SensorType bandClientSensor = GetSensor(bandClient);

            // if we're currently configured with the sensor from the client, do nothing
            if (ReferenceEquals(_sensor, bandClientSensor))
            {
                return;
            }

            // if we have a sensor that isn't from the client that was passed in, tear it down so that we do not continue to receive readings from it.
            if (_sensor != null)
            {
                // the tear-down seems to hang unpredictably. start a background task to prevent lock-ups.
                Task.Run(async () =>
                {
                    // disconnect reading callback so we no longer get readings if the sensor doesn't stop for some reason.
                    try
                    {
                        _sensor.ReadingChanged -= ReadingChangedAsync;
                    }
                    catch (Exception ex)
                    {
                        SensusServiceHelper.Get().Logger.Log("Failed to remove reading-changed handler when reconfiguring probe:  " + ex.Message, LoggingLevel.Normal, GetType());
                    }

                    // tell the sensor to stop reading. this unregisters a low-level handler under the hood.
                    try
                    {
                        await _sensor.StopReadingsAsync();
                    }
                    catch (Exception ex)
                    {
                        SensusServiceHelper.Get().Logger.Log("Failed to stop readings when tearing down sensor for probe reconfigure:  " + ex.Message, LoggingLevel.Normal, GetType());
                    }
                });
            }

            // set up the new sensor
            _sensor = bandClientSensor;
            _sensor.ReadingChanged += ReadingChangedAsync;
        }

        protected abstract SensorType GetSensor(BandClient bandClient);

        protected virtual void ReadingChangedAsync(object sender, BandSensorReadingEventArgs<ReadingType> args)
        {
            Task.Run(async () =>
            {
                try
                {
                    Datum datum = GetDatumFromReading(args.SensorReading);
                    await StoreDatumAsync(datum);
                }
                catch (Exception ex)
                {
                    SensusServiceHelper.Get().Logger.Log("Error getting/storing Band datum:  " + ex.Message, LoggingLevel.Normal, GetType());
                }
            });
        }

        protected override Task StartReadingsAsync()
        {
            if (_sensor == null)
            {
                throw new Exception("Sensor not configured.");
            }
            else
            {
                return _sensor.StartReadingsAsync(SamplingRate);
            }
        }

        protected abstract Datum GetDatumFromReading(ReadingType reading);

        protected override Task StopReadingsAsync()
        {
            if (_sensor == null)
            {
                throw new Exception("Sensor not configured.");
            }
            else
            {
                return _sensor.StopReadingsAsync();
            }
        }
    }
}