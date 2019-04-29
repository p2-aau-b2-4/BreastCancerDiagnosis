using System;
using System.Collections.Generic;
using System.Xml;
using MathNet.Numerics;
using System.Xml.Serialization;
using System.IO;
using System.Numerics;

namespace DimensionReduction
{
    public class Model
    {
        private List<Complex> _eigenValues = new List<Complex>();
        private List<MathNet.Numerics.LinearAlgebra.Vector<double>> _eigenVectors = new List<MathNet.Numerics.LinearAlgebra.Vector<double>>();
        private List<(double, MathNet.Numerics.LinearAlgebra.Vector<double>)> _eigenLumps = new List<(double, MathNet.Numerics.LinearAlgebra.Vector<double>)>();
        private List<List<double>> _features = new List<List<double>>(); 
        private List<MathNet.Numerics.LinearAlgebra.Vector<double>> _meanSums = new List<MathNet.Numerics.LinearAlgebra.Vector<double>>();

        public List<Complex> EigenValues { get => _eigenValues; }
        public List<MathNet.Numerics.LinearAlgebra.Vector<double>> EigenVectors { get => _eigenVectors; }
        public List<(double, MathNet.Numerics.LinearAlgebra.Vector<double>)> EigenLumps { get => _eigenLumps; }
        public List<List<double>> Features { get => _features; }
        public List<MathNet.Numerics.LinearAlgebra.Vector<double>> MeanSums { get => _meanSums; }

        public void SaveModelToFile(string filePath) 
        {
            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(this.GetType());
                writer = new StreamWriter(filePath, false);
                serializer.Serialize(writer, this);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        public void LoadModelFromFile(string filePath) 
        {
            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(this.GetType());
                reader = new StreamReader(filePath);
                Model loadedModel = (Model) serializer.Deserialize(reader);

                this._eigenValues = loadedModel.EigenValues;
                this._eigenVectors = loadedModel.EigenVectors;
                this._eigenLumps = loadedModel.EigenLumps;
                this._features = loadedModel.Features;
                this._meanSums = loadedModel.MeanSums;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        public Model(List<double> eigenValues, List<MathNet.Numerics.LinearAlgebra.Vector<double>> eigenVectors,
                List<(double, MathNet.Numerics.LinearAlgebra.Vector<double>)> eigenLumps,
                List<List<double>> features) {
            _eigenValues = eigenValues;
            _eigenVectors = eigenVectors;
            _eigenLumps = eigenLumps;
            _features = features;

        }
    }
}
