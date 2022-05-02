using System;

namespace Abstractions.UI
{
    public interface ICryptography
    {
        bool ZeroProof { get; }
        string PrivateKey { set; }
        string PublicKey { get; set; }
    }
}
