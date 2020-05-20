namespace Sniffer.Model
{
	public class ActivationLayer : Layer
	{
		// Layer's neurons accessor 
		/// <param name="index">Neuron index</param>
		// Allows to access layer's neurons.
		public new ActivationNeuron this[int index]
		{
			get { return (ActivationNeuron)neurons[index]; }
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="ActivationLayer"/> class
		/// </summary>
		/// <param name="neuronsCount">Layer's neurons count</param>
		/// <param name="inputsCount">Layer's inputs count</param>
		/// <param name="function">Activation function of neurons of the layer</param>
		/// 
		/// <remarks>The new layer will be randomized (see <see cref="ActivationNeuron.Randomize"/>
		/// method) after it is created.</remarks>
		/// 
		public ActivationLayer(int neuronsCount, int inputsCount, IActivationFunction function)
							: base(neuronsCount, inputsCount)
		{
			// create each neuron
			for (int i = 0; i < neuronsCount; i++)
				neurons[i] = new ActivationNeuron(inputsCount, function);
		}
	}
}
