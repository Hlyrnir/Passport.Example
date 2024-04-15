//using Mediator;
//using Microsoft.Extensions.Logging;

//namespace Application.Validation
//{
//	// see https://github.com/martinothamar/Mediator/blob/main/samples/ASPNET_Core_CleanArchitecture/AspNetCoreSample.Application/Pipeline/ErrorLoggingBehaviour.cs

//	public sealed class MessageExceptionBehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
//		where TMessage : notnull, IMessage
//	{
//		private readonly ILogger<MessageExceptionBehaviour<TMessage, TResponse>> lgrError;

//		public MessageExceptionBehaviour(ILogger<MessageExceptionBehaviour<TMessage, TResponse>> lgrError)
//		{
//			this.lgrError = lgrError;
//		}

//		public async ValueTask<TResponse> Handle(TMessage msgMessage, CancellationToken tknCancellation, MessageHandlerDelegate<TMessage, TResponse> dlgMessageHandler)
//		{
//			try
//			{
//				return await dlgMessageHandler(msgMessage, tknCancellation);
//			}
//			catch (Exception exException)
//			{
//				//	// see https://www.youtube.com/watch?v=a26zu-pyEyg -> High-performance logging in .NET, the proper way
//				if (lgrError.IsEnabled(LogLevel.Debug))
//					lgrError.LogError(exException, "Error handling message of type {msgType}", msgMessage.GetType().Name);

//				return await ValueTask.FromException<TResponse>(exException);
//			}
//		}
//	}
//}