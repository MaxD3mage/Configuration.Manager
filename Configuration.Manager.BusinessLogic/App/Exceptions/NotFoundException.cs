namespace Configuration.Manager.BusinessLogic.App.Exceptions;

public class NotFoundException(string message) : BusinessException(message);