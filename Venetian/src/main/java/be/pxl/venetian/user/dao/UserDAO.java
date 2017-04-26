package be.pxl.venetian.user.dao;

import be.pxl.venetian.user.model.User;

import java.util.List;

/**
 * Created by Stef on 22/04/2017.
 */
public interface UserDAO {
    public List<User> getAll();
}

