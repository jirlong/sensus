% Generated by roxygen2: do not edit by hand
% Please edit documentation in R/SensusR.R
\name{plot.AccelerometerDatum}
\alias{plot.AccelerometerDatum}
\title{Plot accelerometer data.}
\usage{
\method{plot}{AccelerometerDatum}(x, pch = ".", type = "l", ...)
}
\arguments{
\item{x}{Accelerometer data.}

\item{pch}{Plotting character.}

\item{type}{Line type.}

\item{...}{Other plotting parameters.}
}
\description{
Plot accelerometer data.
}
\examples{
data.path = system.file("extdata", "example_data", package="SensusR")
data = sensus.read.json.files(data.path)
plot(data$AccelerometerDatum)
}
