package be.pxl.basicsecurity.keypair;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.security.KeyPair;
import java.security.KeyPairGenerator;
import java.security.NoSuchAlgorithmException;
import java.security.NoSuchProviderException;
import java.security.PrivateKey;
import java.security.PublicKey;

public class GenerateKeys {

    private KeyPairGenerator keyGen;
    private KeyPair pair;
    private PrivateKey privateKey;
    private PublicKey publicKey;

    public GenerateKeys(int keylength)
            throws NoSuchAlgorithmException, NoSuchProviderException {

        this.keyGen = KeyPairGenerator.getInstance("RSA");
        this.keyGen.initialize(keylength);

    }

    public void createKeys() {

        this.pair = this.keyGen.generateKeyPair();
        this.privateKey = pair.getPrivate();
        this.publicKey = pair.getPublic();

    }

    public PrivateKey getPrivateKey() {
        return this.privateKey;
    }

    public PublicKey getPublicKey() {
        return this.publicKey;
    }

    public void writeToFile(String path, byte[] key) throws IOException {

        File f = new File(path);
        f.getParentFile().mkdirs();

        FileOutputStream fos = new FileOutputStream(f);
        fos.write(key);
        fos.flush();
        fos.close();

    }

    public static void main(String[] args)
            throws NoSuchAlgorithmException, NoSuchProviderException, IOException {

        GenerateKeys gk_Alice;
        GenerateKeys gk_Bob;

        gk_Alice = new GenerateKeys(1024);
        gk_Alice.createKeys();
        gk_Alice.writeToFile("KeyPair/publicKey_Alice", gk_Alice.getPublicKey().getEncoded());
        gk_Alice.writeToFile("KeyPair/privateKey_Alice", gk_Alice.getPrivateKey().getEncoded());

        gk_Bob = new GenerateKeys(1024);
        gk_Bob.createKeys();
        gk_Bob.writeToFile("KeyPair/publicKey_Bob", gk_Bob.getPublicKey().getEncoded());
        gk_Bob.writeToFile("KeyPair/privateKey_Bob", gk_Bob.getPrivateKey().getEncoded());

    }

}
