using System;
using System.Collections.Generic;
using UnityEngine;


namespace MageAFK.Management
{
  public static class ServiceLocator
  {
    private static readonly IDictionary<Type, object> services = new Dictionary<Type, object>();

    public static void RegisterService<TService>(TService newService)
    {
      Type type = typeof(TService);
      if (!services.ContainsKey(type))
        services[typeof(TService)] = newService;
      else
        Debug.Log($"{type} already added!");
    }

    public static void RemoveService<TService>() => services.Remove(typeof(TService));

    public static bool TryGet<TService>(out TService service, bool tryCreate = true)
    {
      Type serviceType = typeof(TService);
      if (services.ContainsKey(serviceType))
      {
        service = (TService)services[serviceType];
        return true;
      }
      else if (tryCreate)
      {
        if (serviceType.IsClass && !typeof(MonoBehaviour).IsAssignableFrom(serviceType) && !serviceType.IsAbstract)
        {
          // If the type is a concrete class (not an interface or abstract class), create a new instance
          service = Activator.CreateInstance<TService>();
          RegisterService(service);
          return true;
        }

        throw new InvalidOperationException($"Service of type {typeof(TService).Name} not registered or found in hierarchy.");
      }
      service = default;
      Debug.LogWarning($"Service of {typeof(TService).Name} not found");
      return false;
    }

    public static TService Get<TService>()
    {
      if (TryGet<TService>(out var service))
      {
        return service;
      }
      throw new InvalidOperationException($"Service of type {typeof(TService).Name} not registered or found in hierarchy.");
    }
  }
}