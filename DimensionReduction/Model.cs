using System;
using System.Collections.Generic;
using System.Xml;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System.Xml.Serialization;
using System.IO;

namespace DimensionReduction
{
    public class Model
    {
        private List<double> _eigenValues = new List<double>();
        private List<Vector<double>> _eigenVectors = new List<Vector<double>>();
        private List<(double, Vector<double>)> _eigenLumps = new List<(double, Vector<double>)>();
        private List<List<double>> _features = new List<List<double>>(); 

        public List<double> EigenValues { get => _eigenValues; }
        public List<Vector<double>> EigenVectors { get => _eigenVectors; }
        public List<(double, Vector<double>)> EigenLumps { get => _eigenLumps; }
        public List<List<double>> Features { get => _features; }

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
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        public Model(List<double> eigenValues, List<Vector<double>> eigenVectors,
                List<(double, Vector<double>)> eigenLumps,
                List<List<double>> features) {

        }
    }
}
