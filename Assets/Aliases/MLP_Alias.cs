using System;

namespace MultiLayerPerceptron
{
    public class MLP : MultiLayerPerceptron{
        public MLP(int [] Model_Structure, string Model_File_Path="", bool Load_Model_File=true,bool SaveOn_GarbageCollector=true)
         : base(Model_Structure,Model_File_Path,Load_Model_File,SaveOn_GarbageCollector){
             //To do Nothing
         }
   }
}