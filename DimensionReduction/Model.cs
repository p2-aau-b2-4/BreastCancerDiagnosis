using System;
using System.Collections.Generic;
using System.Xml;
using MathNet.Numerics;
using System.Xml.Serialization;
using System.IO;
using System.Numerics;

namespace DimensionReduction
{
    ///<summary>
    ///A model holding PCA training data
    ///</summary>
    public class Model
    {
        private List<Complex> _eigenValues = new List<Complex>();
        private List<MathNet.Numerics.LinearAlgebra.Vector<double>> _eigenVectors = new List<MathNet.Numerics.LinearAlgebra.Vector<double>>();
        private List<(Complex, MathNet.Numerics.LinearAlgebra.Vector<double>)> _eigenLumps = new List<(double, MathNet.Numerics.LinearAlgebra.Vector<double>)>();
        private List<List<double>> _features = new List<List<double>>(); 
        private List<MathNet.Numerics.LinearAlgebra.Vector<double>> _meanSums = new List<MathNet.Numerics.LinearAlgebra.Vector<double>>();

        ///<summary>
        ///Holds all eigen values
        ///</summary>
        public List<Complex> EigenValues { get => _eigenValues; }
        ///<summary>
        ///Holds all eigen vectors
        ///</summary>
        public List<MathNet.Numerics.LinearAlgebra.Vector<double>> EigenVectors { get => _eigenVectors; }
        ///<summary>
        ///Holds value/vector pairs
        ///</summary>
        public List<(Complex, MathNet.Numerics.LinearAlgebra.Vector<double>)> EigenLumps { get => _eigenLumps; }
        ///<summary>
        ///Holds features
        ///</summary>
        public List<List<double>> Features { get => _features; }
        ///<summary>
        ///Holds mean sums
        ///</summary>
        public List<MathNet.Numerics.LinearAlgebra.Vector<double>> MeanSums { get => _meanSums; }

        ///<summary>
        ///Saves the current model object to an XML file.
        ///</summary>
        ///<param name=filePath>relative or absolute path of save file</param>
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

        ///<summary>
        ///Loads a saved model from an XML file.
        ///</summary>
        ///<param name=filePath>relative or absolute path of saved model</param>
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

        ///<summary>
        ///Constructor for model
        ///</summary>
        ///<param name=eigenValues>List of eigen values</param>
        ///<param name=eigenVectors>List of eigen vectors</param>
        ///<param name=eigenLumps>List of eigen value/vector pairs</param>
        ///<param name=features>List of features</param>
        ///<param name=meanSums>List of mean sums</param>
        public Model(List<Complex> eigenValues, List<MathNet.Numerics.LinearAlgebra.Vector<double>> eigenVectors,
                List<(Complex, MathNet.Numerics.LinearAlgebra.Vector<double>)> eigenLumps,
                List<List<double>> features, List<double> meanSums) {
            _eigenValues = eigenValues;
            _eigenVectors = eigenVectors;
            _eigenLumps = eigenLumps;
            _features = features;
            _meanSums = meanSums;
        }
    }
}
