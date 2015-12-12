using System;

namespace ContourAnalysisNS
{
    public class TemplateFinder
    {
        public double minACF = 0.96d;
        public double minICF = 0.85d;
        public bool checkICF = true;
        public bool checkACF = true;
        public double maxRotateAngle = Math.PI;
        public int maxACFDescriptorDeviation = 4;
        public string antiPatternName = "antipattern";

        public FoundTemplateDesc FindTemplate(Templates templates, Template sample)
        {
            //int maxInterCorrelationShift = (int)(templateSize * maxRotateAngle / Math.PI);
            //maxInterCorrelationShift = Math.Min(templateSize, maxInterCorrelationShift+13);
            double rate = 0;
            double angle = 0;
            Complex interCorr = default(Complex);
            Template foundTemplate = null;
            foreach (var template in templates)
            {
                //
                if (Math.Abs(sample.autoCorrDescriptor1 - template.autoCorrDescriptor1) > maxACFDescriptorDeviation) continue;
                if (Math.Abs(sample.autoCorrDescriptor2 - template.autoCorrDescriptor2) > maxACFDescriptorDeviation) continue;
                if (Math.Abs(sample.autoCorrDescriptor3 - template.autoCorrDescriptor3) > maxACFDescriptorDeviation) continue;
                if (Math.Abs(sample.autoCorrDescriptor4 - template.autoCorrDescriptor4) > maxACFDescriptorDeviation) continue;
                //
                double r = 0;
                if (checkACF)
                {
                    r = template.autoCorr.NormDot(sample.autoCorr).Norma;
                    if (r < minACF)
                        continue;
                }
                
                if (template.preferredAngleNoMore90 && Math.Abs(interCorr.Angle) >= Math.PI / 2)
                    continue;//unsuitable angle
                //find max rate
                if (r >= rate)
                {
                    rate = r;
                    foundTemplate = template;
                    angle = interCorr.Angle;
                }
            }
            if (foundTemplate != null)
                return new FoundTemplateDesc() { template = foundTemplate, rate = rate, sample = sample, angle = angle };
            else
                return null;
        }
    }
}