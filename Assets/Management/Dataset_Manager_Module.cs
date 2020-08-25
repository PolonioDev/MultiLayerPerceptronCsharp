using System.Collections.Generic;
using System;
using System.IO;

namespace NeuronalNetwok
{
    namespace Dataset_Manager
    {
        public class Dataset
        {
            public double min_value;
            public double max_value;
            public int inputs_count;
            public int answers_count;
            public long examples_count;
            private bool min_value_selected;
            private bool max_value_selected;
            public long current_examples_count;
            public bool shuffle_items;
            public List<double[]> inputs;
            public List<double[]> answers;

            // File Manager
            public FileStream file;
            public BufferedStream buffer;
            public StreamReader dataset;
            public long buffer_size;
            public Dataset(string csv_file, int inputs_count, int answers_count, int buffer_size=0, bool shuffle_items=true)
            {
                this.buffer_size = buffer_size;
                this.min_value = 0;
                this.max_value = 1;
                this.shuffle_items = shuffle_items;
                this.inputs_count = inputs_count;
                this.answers_count = answers_count;
                this.examples_count = 0;
                this.min_value_selected = false;
                this.max_value_selected = false;
                this.inputs = new List<double[]>();
                this.answers = new List<double[]>();
                try
                {
                    csv_file = Path.GetFullPath(csv_file);
                    if(Path.IsPathRooted(csv_file)){
                        this.file = File.Open(csv_file,FileMode.Open,FileAccess.Read);
                        this.buffer = new BufferedStream(file);
                        this.dataset = new StreamReader(buffer);
                        this.getNext();
                        if(shuffle_items){
                            this.randomize_examples();
                        }
                    }
                }
                catch (System.Exception e)
                {
                    throw new Exception("DATASET: "+e.Message);
                }
            }
            public Dataset(string csv_file, int inputs_count, int answers_count, bool shuffle_items=true)
            :this(csv_file,inputs_count,answers_count,0,shuffle_items){
                // To do nothing
            }
            public Dataset(string csv_file, int inputs_count, int answers_count)
            :this(csv_file,inputs_count,answers_count,0,true){
                // To do nothing
            }
            public void normalizeAll()
            {
                for (int i = 0; i < inputs.Count; i++)
                {
                    for (int j = 0; j < inputs[i].Length; j++)
                    {
                        this.inputs[i][j] = this.normalize(this.inputs[i][j]);
                        this.answers[i][j] = this.normalize(this.answers[i][j]);
                    }
                }
            }
            public void getNext(){
                this.current_examples_count = 0;
                string line;
                while((line = dataset.ReadLine()) != null && (current_examples_count <= buffer_size || buffer_size <= 0)){
                    string[] data = line.Split(",");
                    if(data.Length == 1)
                        continue;
                    double[] input = new double[inputs_count];
                    double[] answer = new double[answers_count];
                    if(data.Length == (inputs_count+answers_count)){
                        for (int i = 0; i < inputs_count; i++)
                        {
                            input[i] = double.Parse(data[i]);
                            if(this.min_value > input[i] || !min_value_selected){
                                this.min_value = input[i];
                                min_value_selected = true;
                            }
                                
                            if(this.max_value < input[i] || !max_value_selected){
                                this.max_value = input[i];
                                max_value_selected = true;
                            }
                        }
                        for (int i = inputs_count; i < data.Length; i++)
                        {
                            answer[i-inputs_count] = double.Parse(data[i]);
                            if(this.min_value > answer[i-inputs_count])
                                this.min_value = answer[i-inputs_count];
                            if(this.max_value < answer[i-inputs_count])
                                this.max_value = answer[i-inputs_count];
                        }
                        
                        inputs.Add(input);
                        answers.Add(answer);
                        this.current_examples_count++;
                        this.examples_count++;
                    }else{
                        throw new Exception("DATASET: The count of inputs and answers (desired outputs) isn't match with this Dataset File.");
                    }
                }
                if(this.shuffle_items){
                    this.randomize_examples();
                }
            }
            public double normalize(double value)
            {
                return (value-min_value)/(max_value-min_value);
            }
            public double unnormalize(double value)
            {
                return value*(max_value-min_value)+min_value;
            }
            public void randomize_examples()
            {
                Random random = new Random(Guid.NewGuid().GetHashCode());
                List<double[]> newInputs = new List<double[]>();
                List<double[]> newAnswers = new List<double[]>();

                for (int i = 0; i < examples_count; i++)
                {
                    int id = (inputs.Count == 1)? 0 : random.Next(0,inputs.Count-1);
                    newInputs.Add(inputs[id]);
                    newAnswers.Add(answers[id]);
                    this.inputs.Remove(inputs[id]);
                    this.answers.Remove(answers[id]);
                }
                this.inputs = newInputs;
                this.answers = newAnswers;
            }
            ~Dataset(){
                file.Close();
                buffer.Close();
                dataset.Close();
            }
        }
    }
}