#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using Macro.Common;
using Macro.Desktop.Actions;
using Macro.ImageViewer.BaseTools;
using Macro.ImageViewer.Graphics;
using Macro.ImageViewer.VtkItkAdapters;
using itk;
using CastImageFilterType = itk.itkCastImageFilter;
using SmoothingFilterType = itk.itkCurvatureAnisotropicDiffusionImageFilter;
using GradientMagnitudeFilterType = itk.itkGradientMagnitudeRecursiveGaussianImageFilter;
using SigmoidFilterType = itk.itkSigmoidImageFilter;
using FastMarchingFilterType = itk.itkFastMarchingImageFilter;
using BinaryThresholdFilterType = itk.itkBinaryThresholdImageFilter;
using intensityFilterType = itk.itkRescaleIntensityImageFilter;

namespace Macro.ImageViewer.Tools.ImageProcessing.Filter
{
	[MenuAction("apply", "global-menus/MenuTools/MenuFilter/MenuFastMarchingSegmentation", "Apply")]
    [MenuAction("apply", "imageviewer-filterdropdownmenu/MenuFastMarchingSegmentation", "Apply")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class FastMarchingSegmenatationTool : ImageViewerTool
	{
        public FastMarchingSegmenatationTool()
		{

		}

		public void Apply()
		{
			if (this.SelectedImageGraphicProvider == null)
				return;

			ImageGraphic image = this.SelectedImageGraphicProvider.ImageGraphic;

			if (image == null)
				return;

			if (!(image is GrayscaleImageGraphic))
				return;

            itkImageBase input = ItkHelper.CreateItkImage(image as GrayscaleImageGraphic);
            itkImageBase output = itkImage.New(input);
            ItkHelper.CopyToItkImage(image as GrayscaleImageGraphic, input);

            String mangledType = input.MangledTypeString;
            CastImageFilterType castToIF2 = CastImageFilterType.New(mangledType + "IF2");

            SmoothingFilterType smoothingFilter = SmoothingFilterType.New("IF2IF2");
            smoothingFilter.TimeStep = 0.125;
            smoothingFilter.NumberOfIterations = 5;
            smoothingFilter.ConductanceParameter = 9.0;

            GradientMagnitudeFilterType gradientMagnitudeFilter = GradientMagnitudeFilterType.New("IF2IF2");
            gradientMagnitudeFilter.Sigma = 1.0;

            SigmoidFilterType sigmoidFilter = SigmoidFilterType.New("IF2IF2");
            sigmoidFilter.OutputMinimum = 0.0;
            sigmoidFilter.OutputMaximum = 1.0;
            sigmoidFilter.Alpha = -0.5;//-0.3
            sigmoidFilter.Beta = 3.0;//2.0

            FastMarchingFilterType fastMarchingFilter = FastMarchingFilterType.New("IF2IF2");
            double seedValue = 0.0;
            int[] seedPosition = {256, 256};// user input
            itkIndex seedIndex = new itkIndex(seedPosition);
            itkLevelSetNode[] trialPoints = { new itkLevelSetNode(seedValue, seedIndex) };
            fastMarchingFilter.TrialPoints = trialPoints;
            fastMarchingFilter.StoppingValue = 100;

            BinaryThresholdFilterType binaryThresholdFilter = BinaryThresholdFilterType.New("IF2" + mangledType);//to UC2?
            binaryThresholdFilter.UpperThreshold = 100.0;//200
            binaryThresholdFilter.LowerThreshold = 0.0;
            binaryThresholdFilter.OutsideValue = 0;
            if (image.BitsPerPixel == 16)
                binaryThresholdFilter.InsideValue = (image as GrayscaleImageGraphic).ModalityLut.MaxInputValue;//32767;
            else
                binaryThresholdFilter.InsideValue = 255;

            //intensityFilterType intensityFilter = intensityFilterType.New("UC2" + mangledType);
            //intensityFilter.OutputMinimum = 0;
            //if (image.BitsPerPixel == 16)
            //    intensityFilter.OutputMaximum = (image as GrayscaleImageGraphic).ModalityLut.MaxInputValue;//32767;
            //else
            //    intensityFilter.OutputMaximum = 255;

            // Make data stream connections
            castToIF2.SetInput(input);
            smoothingFilter.SetInput(castToIF2.GetOutput());
            gradientMagnitudeFilter.SetInput(smoothingFilter.GetOutput());
            sigmoidFilter.SetInput(gradientMagnitudeFilter.GetOutput());
            fastMarchingFilter.SetInput(sigmoidFilter.GetOutput());
            binaryThresholdFilter.SetInput(fastMarchingFilter.GetOutput());
            //intensityFilter.SetInput(binaryThresholdFilter.GetOutput());

            //smoothingFilter.Update();
            fastMarchingFilter.OutputSize = input.BufferedRegion.Size;//?
            binaryThresholdFilter.Update();

            binaryThresholdFilter.GetOutput(output);
            ItkHelper.CopyFromItkImage(image as GrayscaleImageGraphic, output);
            image.Draw();

            input.Dispose();
            output.Dispose();
		}
	}
}
