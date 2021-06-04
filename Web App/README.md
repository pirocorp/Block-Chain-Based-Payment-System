## Concepts

Real blockchain implementations such as bitcoin and others, use private keys, public keys, and addresses as identity or proof of ownership.

### Asymmetric Cryptography

> Public-key cryptography, or asymmetric cryptography, is a cryptographic system which uses pairs of keys: public keys (which may be known to others), and private keys (which may never be known by any except the owner). The generation of such key pairs depends on cryptographic algorithms which are based on mathematical problems termed one-way functions. Effective security requires keeping the private key private; the public key can be openly distributed without compromising security.

[Public-key cryptography](https://en.wikipedia.org/wiki/Public-key_cryptography)
![Asymetric Encryption](./Asymetric_Encryption.png)

How do you prove that blockchain transactions are yours? The answer is the signature. In the cryptocurrency world, asymmetric cryptography is used to make signatures of messages (transactions) that are sent from the wallet to the server (blockchain node) and verify that the messages were sent by the original sender.
