using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Stylet;
using System;
using System.ComponentModel;

namespace MongoDbGenericRepository.Models
{
    /// <summary>
    /// This class represents a basic document that can be stored in MongoDb.
    /// Your document must implement this class in order for the MongoDbRepository to handle them.
    /// </summary>
    /// 
    [BsonIgnoreExtraElements(true,Inherited =true)]
    public   class Document : PropertyChangedBase, IDocument
    {
         

        /// <summary>
        /// The document constructor
        /// </summary>
        public Document()
        {
            Id = ObjectId.GenerateNewId();
            AddedAtUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// The Id of the document
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }

        /// <summary>
        /// The datetime in UTC at which the document was added.
        /// </summary>
        /// 
        
        public virtual DateTime AddedAtUtc { get; set; }

        /// <summary>
        /// The version of the schema of the document
        /// </summary>
        public int Version { get; set; }

        [BsonIgnore]
        public virtual string Name { get; set; }
    }
}