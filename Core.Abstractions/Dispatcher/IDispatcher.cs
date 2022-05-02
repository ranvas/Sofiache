using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Abstractions.Dispatcher
{
    public interface IDispatcher
    {
        /// <summary>
        /// Простая обработка для любых типа запроса и типа ответа
        /// </summary>
        /// <typeparam name="TIn">Тип запроса</typeparam>
        /// <typeparam name="TOut">Тип ожидаемого ответа</typeparam>
        /// <param name="request">Запрос</param>
        /// <returns>Ответ</returns>
        Task<TOut?> DispatchSimple<TIn, TOut>(TIn request);

        /// <summary>
        /// Простая именованная обработка для пустого запроса и любого типа ответа
        /// </summary>
        /// <param name="key">Ключ обработчика</param>
        /// <returns>Ответ</returns>
        Task<TOut?> DispatchSimple<TOut>(string key);

        /// <summary>
        /// Простая именованная обработка для любых типа запроса и типа ответа
        /// </summary>
        /// <typeparam name="TIn">Тип запроса</typeparam>
        /// <typeparam name="TOut">Тип ожидаемого ответа</typeparam>
        /// <param name="key">Ключ обработчика</param>
        /// <param name="request">Ответ</param>
        /// <returns></returns>
        Task<TOut?> DispatchSimple<TIn, TOut>(string key, TIn request);

        /// <summary>
        /// Обработка запроса используя специфику зарегистрированных обработчиков типа запроса и типа ответа
        /// </summary>
        /// <typeparam name="TIn">Тип запроса, ограниченный интерфейсом IDispatchedRequest</typeparam>
        /// <typeparam name="TOut">Тип ожидаемого ответа, ограниченный интерфейсом IDispatchedResponse</typeparam>
        /// <param name="request">Запрос, ограниченный интерфейсом IDispatchedRequest</param>
        /// <returns>Ответ, ограниченный интерфейсом IDispatchedResponse</returns>
        Task<TOut?> DispatchSpecified<TIn, TOut>(TIn request) where TIn : IDispatchedRequest where TOut : IDispatchedResponse;
    }
}
