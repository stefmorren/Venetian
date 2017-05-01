package be.pxl.venetian.user.model;

/**
 * Created by Stef on 22/04/2017.
 */
public class User {
    private int id;
    private String username;
    private String password;

    public String getUsername() {
        return username;
    }

    public void setUsername(String username) {
        this.username = username;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public User(int id, String username, String password) {
        this.username = username;
        this.password = password;
    }

    public User() {
    }

    public User(String username) {
        this.username = username;
    }
}
