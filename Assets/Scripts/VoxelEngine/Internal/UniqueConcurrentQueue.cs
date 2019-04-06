using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

public class UniqueConcurrentQueue<T> {
    private HashSet<T> hashSet;
    private ConcurrentQueue<T> queue;

    public UniqueConcurrentQueue() {
        hashSet = new HashSet<T>();
        queue = new ConcurrentQueue<T>();
    }

    public int Count {
        get => hashSet.Count;
    }

    public bool Contains(T item) {
        return hashSet.Contains(item);
    }

    public void Enqueue(T item) {
        if (hashSet.Add(item)) {
            queue.Enqueue(item);
        }
    }

    public bool Dequeue(out T result) {
        bool success = queue.TryDequeue(out result);
        if (success) hashSet.Remove(result);
        return success;
    }

    public bool Peek(out T result) {
        return queue.TryPeek(out result);
    }
}